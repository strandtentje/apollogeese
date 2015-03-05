using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Navigation
{
	/// <summary>
	/// Site subsection based on URL.
	/// </summary>
	public class SiteSubsection : Service
	{
		public override string Description {
			get {
				return string.Join(", ", Branches.Dictionary.Keys);
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["main"] = Stub;
			Branches ["default"] = Stub;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction httpParameters = (IHttpInteraction)uncastParameters.GetClosest(typeof(IHttpInteraction));

			SubsectionInteraction interaction = new SubsectionInteraction (httpParameters, uncastParameters);

			Service branch = Stub;

			if (branch == Stub) 
				branch = Branches [interaction.BranchName] ?? Stub;
			
			if (branch == Stub)
				branch = Branches ["default"];
			else
				interaction.Confirm ();

			return branch.TryProcess (interaction);
		}
	}
}

