using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Simple template service which fills 
	/// </summary>
	public class Template : Service
	{
		/// <summary>
		/// Regex Matches that may be replaced
		/// </summary>
		private MatchCollection replaceables;
		/// <summary>
		/// Variables this template supports
		/// </summary>
		private StringList templateVariables = new StringList();

		private string templateFile;

		[Instruction("Internal title of this template", "untitled")]
		public string Title { get; set; }

		[Instruction("Placeholder pattern", @"\{% ([a-z|_|\.]+) %\}")]
		public string PlaceholderPattern { get; set; }

		[Instruction("Absolute path to template file")]
		public string TemplateFile { 
			get {
				return this.templateFile;
			}
			set {
				this.templateFile = value;
				LoadTemplateAndRegisterReplacableSegments();
			}
		}

		[Instruction("Filler text to appear when the value couldn't be acquired from context.", "")]
		public string MissingContextFiller;

		[Instruction("Filler text to appear when the value couldn't be acquired by means of branch invocation.", "")]
		public string MissingBranchFiller;

		[Instruction("When set to true, Template engine will reload template file when modified", true)]
		public bool WillCheckForTemplateUpdates { get; private set; }

		[Instruction("Will check existing context before invoking branch by placeholder name", false)]
		public bool ChecksContextFirst { 
			get {
				return WriteElement == WriteContextFirst;
			} set {
				if (value)
					WriteElement = WriteContextFirst;
				else
					WriteElement = WriteBranchFirst;
			}
		}

		private string rawTemplate;

		public DateTime LastTemplateUpdate { get; private set; }

		/// <summary>
		/// Returns the title with this service.
		/// </summary>
		/// <value>The title of this page</value>
		public override string Description {
			get { return string.Format("{0} ({1})", Title, TemplateFile); }
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["templatefile"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			Title = modSettings.GetString ("title", "untitled");
			PlaceholderPattern = modSettings.GetString ("chunkpattern", @"\{% ([a-z|_|\.]+) %\}");
			WillCheckForTemplateUpdates = modSettings.GetBool("checkfortemplateupdates", true);
			ChecksContextFirst = modSettings.GetBool ("contextfirst", false);
			MissingBranchFiller = modSettings.GetString("forwardfail", "");
			MissingContextFiller = modSettings.GetString("backwardfail",  "");
			TemplateFile = modSettings.GetString ("templatefile");
		}

		private bool HasTemplateUpdates(bool reset = true) {
			FileInfo info = new FileInfo (TemplateFile);

			if (LastTemplateUpdate != info.LastWriteTime) {
				if (reset) 
					LastTemplateUpdate = info.LastWriteTime;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks for template updates.
		/// </summary>
		private void LoadTemplateUpdates ()
		{
			if (HasTemplateUpdates()) {
				LoadTemplateAndRegisterReplacableSegments ();

				Secretary.Report(5, "Template file was updated: ", TemplateFile);
			} 
		}

		/// <summary>
		/// Loads the template and register replacable segments.
		/// </summary>
		private void LoadTemplateAndRegisterReplacableSegments ()
		{
			if (!File.Exists (TemplateFile))
				File.Create (TemplateFile).Close();

			rawTemplate = File.ReadAllText (TemplateFile);

			replaceables = templateVariables.AddUniqueRegexMatches (rawTemplate, PlaceholderPattern);

			foreach(string templateVariable in templateVariables)
				if (!Branches.Has(templateVariable))
					Branches[templateVariable] = Stub;
		}

		delegate void WriteElementDelegate(StreamWriter outputWriter, IInteraction source, string groupName);

		WriteElementDelegate WriteElement;

		void WriteBranchFirst (StreamWriter outputWriter, IInteraction source, string groupName)
		{			
			Service branch = Branches[groupName];

			if ((branch ?? Stub) == Stub) {
				object chunk;
				if (source.TryGetFallback(groupName, out chunk))
				{
					outputWriter.Write(chunk.ToString());
				}
				else 
				{
					outputWriter.Write(string.Format(MissingContextFiller, groupName));
				}
			} else if (!branch.TryProcess(source)) {
				outputWriter.Write(string.Format(MissingBranchFiller, groupName));
			}		
		}

		void WriteContextFirst (StreamWriter outputWriter, IInteraction source, string groupName)
		{			
			object chunk;
			if (source.TryGetFallback (groupName, out chunk)) {
				outputWriter.Write (chunk.ToString ());
			} else {
				Service branch = Branches[groupName];

				if ((branch ?? Stub) == Stub) {
					outputWriter.Write(string.Format(MissingContextFiller, groupName));
				} else if (!branch.TryProcess(source)) {
					outputWriter.Write(string.Format(MissingBranchFiller, groupName));
				}	
			}				
		}

		private string GetSignature(INosyInteraction nosyInteraction) {
			if (nosyInteraction.IncludeContext) {
				StringBuilder signatureBuilder = new StringBuilder ();

				signatureBuilder.Append (this.TemplateFile);
				signatureBuilder.Append (HasTemplateUpdates(false).ToString());
				signatureBuilder.Append (LastTemplateUpdate.ToBinary ());

				foreach (Match replaceable in replaceables) {
					if (WriteElement == WriteContextFirst) {
						string name = replaceable.Groups [1].Value;
						object value;
						if (nosyInteraction.TryGetFallback (name, out value)) {
							signatureBuilder.Append (name);
							signatureBuilder.Append (":");	
							signatureBuilder.AppendLine (value.ToString ());
						}
					}
				}

				return signatureBuilder.ToString ();
			} else {
				return string.Format ("{0}{1}{2}", this.TemplateFile, HasTemplateUpdates (false).ToString (), LastTemplateUpdate.ToBinary());
			}
		}

		protected override bool Process (IInteraction source)
		{
			if (source is INosyInteraction) {
				INosyInteraction nosyInteraction = (INosyInteraction)source;

				nosyInteraction.Signature = GetSignature (nosyInteraction);
			} else {
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

				if (WillCheckForTemplateUpdates) LoadTemplateUpdates();

				StreamWriter outputWriter = target.GetOutgoingBodyWriter ();

				try	{
					foreach (Match replaceable in replaceables) {
						outputWriter.Write(rawTemplate.Substring (cursor, replaceable.Index - cursor));

						groupName = replaceable.Groups[1].Value;

						WriteElement(outputWriter, source, groupName);							

						cursor = replaceable.Index + replaceable.Length;
					}

					// In the very likely event the cursor is not at the end of the document,
					// the last bit of the document needs to be written to the body as well.
					if (cursor < rawTemplate.Length)
						outputWriter.Write(rawTemplate.Substring(cursor));

					outputWriter.Flush();
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
			}

			return true;
		}
	}
}

