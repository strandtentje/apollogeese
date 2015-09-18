using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.IO;

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

			StreamWriter writer = interaction.GetOutgoingBodyWriter ();

			if (VariableName.Length == 0) {
				writer.Write(Format);
			} else if (success = parameters.TryGetFallbackString (VariableName, out text)) {
				writer.Write(string.Format(Format, text));
			}

			writer.Flush ();

			return success;
		}
	}
}

