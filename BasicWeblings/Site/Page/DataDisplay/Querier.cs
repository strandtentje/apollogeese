using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Extensions.BasicWeblings.Server;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	public class Querier : Service
	{
		private Service iterator = Stub;

		public override string Description {
			get {
				return "Queries and iterates with attached service.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches["Iterator"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "Iterator") iterator = e.NewValue;
		}


		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction castInteraction = (IHttpInteraction)parameters;

			if (castInteraction == null) {
				L.Report(4, "Incoming data was of incorrect type");
				return false;
			}


			if (generator.TryProcess (generatorInteraction)) {


			} else {
				L.Report(4, "Unable to generate data from parameters");
				return false;
			}
		}
	}
}

