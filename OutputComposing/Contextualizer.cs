using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class Contextualizer : Service
	{
		public override string Description {
			get {
				return "Contextualize outgoing data into variable";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings["variablename"] = defaultParameter;
		}

		string VariableName;

		Encoding Encoding;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.VariableName = settings.GetString("variablename", "data");
			this.Encoding = Encoding.GetEncoding(settings.GetString("encoding", "utf-8"));
		}

		Service Input, Output;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "input") this.Input = e.NewValue;
			else if (e.Name == "output") this.Output = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			var composer = new StringComposeInteraction(parameters, Encoding);
			var composeResult = Input.TryProcess(composer);
			var outputter = new SimpleInteraction(parameters, VariableName, composer.ToString());
			var outputResult = Output.TryProcess(outputter);

			return composeResult && outputResult;
		}

	}
}

