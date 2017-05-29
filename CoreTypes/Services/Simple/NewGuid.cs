using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace Services.Simple
{
	public class NewGuid : SingleBranchService
	{
		public override string Description {
			get {
				return "Guid Maker";
			}
		}

		public override void LoadDefaultParameters (object defaultParameter)
		{
			Settings["variable"] = defaultParameter;
		}

		string Variable;

		string Format;

		protected override void Initialize (Settings settings)
		{
			this.Variable = settings.GetString("variable", "guid");
			this.Format = settings.GetString("format", "N");
		}

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess(new SimpleInteraction(parameters,Variable, Guid.NewGuid().ToString(this.Format)));
		}
	}
}

