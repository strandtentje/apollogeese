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
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Simple template service which fills 
	/// </summary>
	public class Template : Service
	{
		List<IExpression> expressions = new List<IExpression> ();

		private string templateFile;

		[Instruction("Internal title of this template", "untitled")]
		public string Title { get; set; }

		[Instruction("Absolute path to template file")]
		public string TemplateFile { 
			get {
				return this.templateFile;
			}
			set {
				this.templateFile = value;
				UpdateTemplate();
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
		/// Loads the template and register replacable segments.
		/// </summary>
		private void UpdateTemplate ()
		{
			if (!File.Exists (TemplateFile))
				File.Create (TemplateFile).Close();

			rawTemplate = File.ReadAllText (TemplateFile);

			// me 16: string.Split() ftw lolzors superfast superclear
			// me 19: we should do this neatly using classes and regexes
			//        because i'm an intelligent, skilled programmer
			// me 22: LMAO STRING.SPLIT() FUCK YEAH SOCKS FOR CHRISTMAS
			IEnumerable<string> rawExpressions = rawTemplate.Split ("{%".ToCharArray());
			// to be fair, i can only imagine how much faster String.Split 
			// is compared to regexes.

			char opener;

			foreach (string rawExpression in rawExpressions) {
				if (rawExpression.EndsWith ("%}")) {
					opener = rawExpression [0];

					switch (opener) {
					case '&':
						expressions.Add (new Replacement (rawExpression.Substring(1)));
						break;
					case '*':
						expressions.Add (new Call(rawExpression.Substring(1), this));
						break;
					default:
						expressions.Add (new CallOrReplace (rawExpression, this));
						break;
					}
				} else {
					expressions.Add (new Literal (rawExpression));
				}
			}

			Secretary.Report(5, "Template file was updated: ", TemplateFile);
		}

		public Service Default {
			get {
				return Branches ["default"] ?? Stub;
			}
		}

		protected override bool Process (IInteraction source)
		{
			IOutgoingBodiedInteraction target;
			MimeType type;

			target = (IOutgoingBodiedInteraction)source.GetClosest (typeof(IOutgoingBodiedInteraction));

			if (target is IHttpInteraction) {
				type = MimeType.Text.Html;
				type.Encoding = Encoding.UTF8;
				((IHttpInteraction)target).ResponseHeaders.ContentType = type;
			}

			if (HasTemplateUpdates ()) {
				UpdateTemplate ();
			}

			StreamWriter outputWriter = new StreamWriter (target.OutgoingBody);

			bool successful = true;

			foreach(IExpression expression in expressions) {
				successful &= (
					expression.TryWriteTo (outputWriter, source) || 
					Default.TryProcess (source));
			}

			outputWriter.Flush();

			return successful;
		}
	}
}

