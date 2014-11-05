using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	/// <summary>
	/// Site subsection based on URL.
	/// </summary>
	public class SiteSubsection : Service
	{
		public override string Description {
			get {
				return "A URL-based website subsection. Will branch based on URL chunk, where no URL chunk branches to 'main' and an unknown URL chunk branches to 'default'.";
			}
		}

		/// <summary>
		/// The main service, provided if the URL terminates at this subsection
		/// </summary>
		Service Main;
		/// <summary>
		/// The default service, provided if the URL can't be deciphered at this subsection.
		/// </summary>
		Service Default;

		protected override void Initialize (Settings modSettings)
		{
			Branches ["main"] = Stub;
			Branches ["default"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "main") Main = e.NewValue;
			if (e.Name == "default") Default = e.NewValue;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = uncastParameters as IHttpInteraction;

			Service Branch;

			if (parameters.URL.Count == 0) {
				if (Main == Stub)
					Branch = Default;
				else 
					Branch = Main;

				parameters["branchname"] = "main";
			}
			else {
				string branchName = parameters.URL.Peek();

				parameters["branchname"] = branchName;
				Branch = Branches [branchName] ?? Stub;

				if (Branch == Stub)
					Branch = Default;
				else
					parameters.URL.Dequeue();
			}

			return Branch.TryProcess (uncastParameters); // RunBranch (branchId, parameters);
		}
	}
}

