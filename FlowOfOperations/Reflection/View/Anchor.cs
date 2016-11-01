using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Anchor : SingleBranchService
	{
		string desc;

		public override string Description {
			get {
				return desc;
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["title"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			desc = modSettings.GetString("title", "");
		}

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess(parameters);
		}
	}
}

