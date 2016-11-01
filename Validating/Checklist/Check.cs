using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Validating
{
	public class Check : SingleBranchService
	{
		public override string Description {
			get {
				return "Branch either successful or failure depending on whether or" +
				"not failures were reported downstream on 'subject'";
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess (new CheckInteraction (parameters));
		}
	}
}

