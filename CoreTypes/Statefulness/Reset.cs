using System;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class Reset : SingleBranchService
	{		
		public override string Description {
			get {
				if (WithBranch == null) {
					return "Service Resetter";
				} else {
					return string.Format ("Resetter for '{0}'", WithBranch.Description);
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess (new ResetInteraction (parameters));
		}
	}
}

