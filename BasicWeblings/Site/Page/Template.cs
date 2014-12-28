using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	/// <summary>
	/// Simple template service which fills 
	/// </summary>
	public class Template : Service
	{
		private Settings settings;
		/// <summary>
		/// Regex Matches that may be replaced
		/// </summary>
		private MatchCollection replaceables;
		/// <summary>
		/// Variables this template supports
		/// </summary>
		private StringList templateVariables = new StringList();

		private string templateFile, rawTemplate, chunkPattern, title;
		private string unsuppliedFormat, inacquirableFormat;

		public bool WillCheckForTemplateUpdates { get; private set; }

		public DateTime LastTemplateUpdate { get; private set; }

		/// <summary>
		/// Returns the title with this service.
		/// </summary>
		/// <value>The title of this page</value>
		public override string Description {
			get { return title; }
		}

		protected override void Initialize (Settings modSettings)
		{
			title = (string)modSettings ["title"];
			chunkPattern = modSettings.GetString ("chunkpattern", @"\{% ([a-z|\_]+) %\}");
			templateFile = (string)modSettings ["templatefile"];
			WillCheckForTemplateUpdates = modSettings.GetBool("checkfortemplateupdates", true);
			inacquirableFormat = modSettings.GetString("forwardfail", "");
			unsuppliedFormat = modSettings.GetString("backwardfail",  "");
			settings = modSettings;

			if (WillCheckForTemplateUpdates)
				CheckForTemplateUpdates();
			else
				LoadTemplateAndRegisterReplacableSegments();
		}

		/// <summary>
		/// Checks for template updates.
		/// </summary>
		private void CheckForTemplateUpdates ()
		{
			FileInfo info = new FileInfo(templateFile);

			if ((LastTemplateUpdate == null) || 
			    (LastTemplateUpdate != info.LastWriteTime)) {

				LastTemplateUpdate = info.LastWriteTime;
				LoadTemplateAndRegisterReplacableSegments ();

				Secretary.Report(5, "Template file was updated: ", templateFile);
			} 
		}

		/// <summary>
		/// Loads the template and register replacable segments.
		/// </summary>
		private void LoadTemplateAndRegisterReplacableSegments ()
		{
			if (!File.Exists (templateFile))
				File.Create (templateFile).Close();

			rawTemplate = File.ReadAllText (templateFile);

			replaceables = templateVariables.AddUniqueRegexMatches (rawTemplate, chunkPattern);

			foreach(string templateVariable in templateVariables)
				if (!Branches.Has(templateVariable))
					Branches[templateVariable] = Stub;
		}

		protected override bool Process (IInteraction source)
		{
			IOutgoingBodiedInteraction target;
			MimeType type;
			int cursor = 0;
			string groupName;

			target = (IOutgoingBodiedInteraction)source.GetClosest (typeof(IOutgoingBodiedInteraction));


			if (target is IHttpInteraction) {
				type = MimeType.Text.Html;
				type.Encoding = Encoding.UTF8;
				((IHttpInteraction)target).ResponseHeaders.ContentType = type;
			}

			if (WillCheckForTemplateUpdates) CheckForTemplateUpdates();

			try	{
				foreach (Match replaceable in replaceables) {
					target.OutgoingBody.Write(rawTemplate.Substring (cursor, replaceable.Index - cursor));

					groupName = replaceable.Groups[1].Value;

					Service branch = Branches[groupName];

					if ((branch ?? Stub) == Stub) {
						object chunk;
						if (source.TryGetFallback(groupName, out chunk))
						{
							target.OutgoingBody.Write(chunk.ToString());
						}
						else 
						{
							target.OutgoingBody.Write(string.Format(unsuppliedFormat, groupName));
						}
					} else if (!branch.TryProcess(source)) {
						target.OutgoingBody.Write(string.Format(inacquirableFormat, groupName));
					}				

					cursor = replaceable.Index + replaceable.Length;
				}

				// In the very likely event the cursor is not at the end of the document,
				// the last bit of the document needs to be written to the body as well.
				if (cursor < rawTemplate.Length)
					target.OutgoingBody.Write(rawTemplate.Substring(cursor));

			}
			catch (Exception ex) {
				// Forward the exception but append a cursor position and a message
				// so the user won't be entirely in the dark.
				throw new Exception (
					string.Format (
					"Replacing template variables failed at cursor position {0}, {2}:\n{1}",
					cursor.ToString (), 
					rawTemplate.Substring (cursor, 40), 
					ex.Message), ex);
			}


			return true;
		}
	}
}

