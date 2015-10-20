using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class Write : Service
	{
		public override string Description {
			get {
				return Format;
			}
		}

		[Instruction("Name of context varaible to write into the format (optional)", "")]
		private string VariableName { get; set; }
		[Instruction("Format to write to the OutgoingBody, optionally including {0} as placeholder for variable.", "{0}")]
		private string Format { get; set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["format"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			VariableName = modSettings.GetString("variablename", "");
			Format = modSettings.GetString ("format", "{0}");
		}

		protected override bool Process (IInteraction parameters)
		{
			IOutgoingBodiedInteraction interaction = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));

			bool success = true;
			string text; 

			if (VariableName.Length == 0) {
				text = Format;
			} else if (success = parameters.TryGetFallbackString (VariableName, out text)) {
				text = string.Format(Format, text);
			}

			byte[] data = Encoding.Unicode.GetBytes (text);

			interaction.OutgoingBody.Write(data, 0, data.Length);

			return success;
		}
	}
}

