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
		List<Expression> expressions = new List<Expression> ();

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
		/// Finds the expression for the given replacement name
		/// </summary>
		/// <returns>The expression.</returns>
		/// <param name="replacename">Replacename.</param>
		Expression FindExpression (string replacename)
		{			
			Expression result;

			switch (replacename[0]) {
				case '<':
				result = new Replacement (replacename.Substring(1));
				break;
			case '>':
				result = new Call(replacename.Substring(1), this);
				break;
			default:
				result = new CallOrReplace (replacename, this);
				break;
			}

			return result;
		}

		/// <summary>
		/// Loads the template and register replacable segments.
		/// </summary>
		private void UpdateTemplate ()
		{
			if (!File.Exists (TemplateFile))
				File.Create (TemplateFile).Close();

			rawTemplate = File.ReadAllText (TemplateFile);
						
			expressions.Clear ();

			for (int i = 0; i < rawTemplate.Length;) {
				expressions.Add (Expression.FromString (
					rawTemplate, ref i, FindExpression));
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

			foreach(Expression expression in expressions) {
				successful &= (
					expression.TryWriteTo (outputWriter, source) || 
					Default.TryProcess (source));
			}

			outputWriter.Flush();

			return successful;
		}
	}
}

