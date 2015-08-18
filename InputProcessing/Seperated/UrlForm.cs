using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace InputProcessing
{
	public class UrlForm : Form
	{
		string sourceName;

		bool IsBodySource {
			get;
			set;
		}

		string ContextVariable {
			get;
			set;
		}

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

		protected override IKeyValueReader GetReader (IInteraction parameters)
		{
			IKeyValueReader reader;

			if (IsBodySource) {
				IIncomingBodiedInteraction incomingBody;

				if (parameters.TryGetClosest (
					    typeof(IIncomingBodiedInteraction),
					    out incomingBody)) {
					reader = new UrlKeyValueReader (parameters, 
						incomingBody.GetIncomingBodyReader (),
						this.Source);
				} else {
					throw new Exception (
						"Incoming Body is required when attempting to load from body.");
				}
			} else {
				string data;
				if (parameters.TryGetFallbackString (
					    this.ContextVariable, out data)) {
					reader = new UrlKeyValueReader (parameters,
						StringReader (data), this.Source);
				} else {
					throw new Exception (string.Format(
						"UrlForm was set to load form from context variable {0}, which wasn't present.",
						this.ContextVariable));
				}
			}


		}
	}
}

