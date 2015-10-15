using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace InputProcessing
{
	public abstract class Form : InputListing
	{
		bool IsBodySource {
			get;
			set;
		}

		string ContextVariable {
			get;
			set;
		}

		string sourceName;

		[Instruction("Source for URL-encoded data", "body")]
		public string Source {
			get {
				return this.sourceName;
			}
			set {
				this.sourceName = value;

				IsBodySource = this.sourceName == "body";

				if (!IsBodySource) {
					if (this.sourceName.StartsWith ("context:")) {
						this.ContextVariable = 
							this.sourceName.Substring ("context:".Length);
					} else {
						throw new Exception (
							"Invalid value for 'Source'; use 'body' or " + 
							"anything that starts with 'context:'");
					}
				}
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.Source = settings.GetString ("source", "body");
		}

		TextReader GetBodyTextReader (IInteraction parameters)
		{
			IIncomingReaderInteraction incomingBody;
			IInteraction incomingBodyCandidate;
			TextReader reader;

			if (parameters.TryGetClosest (
				typeof(IIncomingReaderInteraction),
				out incomingBodyCandidate)) {
				incomingBody = (IIncomingReaderInteraction)incomingBodyCandidate;
				reader = incomingBody.GetIncomingBodyReader ();
			} else {
				throw new Exception (
					"Incoming Body is required when attempting to load from body.");
			}

			return reader;
		}

		TextReader GetContextTextReader(IInteraction parameters) {
			TextReader reader;

			string data;
			if (parameters.TryGetFallbackString (
				this.ContextVariable, out data)) {
				reader = new StringReader (data);
			} else {
				throw new Exception (string.Format(
					"UrlForm was set to load form from context variable {0}, which wasn't present."));
			}

			return reader;
		}

		public TextReader GetTextReader (IInteraction parameters)
		{
			if (IsBodySource) {
				return GetBodyTextReader (parameters);
			} else {
				return GetContextTextReader (parameters);
			}
		}
	}
}
