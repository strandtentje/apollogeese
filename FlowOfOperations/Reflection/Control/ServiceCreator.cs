using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ServiceCreator : Service
	{
		public override string Description {
			get {
				return "Instantiate service";
			}
		}

		string ServiceNameKey {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.ServiceNameKey = modSettings.GetString ("servicenamekey", "servicename");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful")
				this.Successful = e.NewValue;
			if (e.Name == "failure") 
				this.Failure = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{

		}
	}
}

