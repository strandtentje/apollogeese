using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class NewService : TwoBranchedService
	{
		public override string Description {
			get {
				return "Instantiate service";
			}
		}

		string ServiceNameKey { get; set; }
		string ServiceIdKey { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			this.ServiceNameKey = modSettings.GetString ("servicenamekey", "servicename");
			this.ServiceIdKey = modSettings.GetString ("serviceidkey", "serviceid");
		}

		/// <summary>
		/// Creates a service by name.
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="serviceName">Service name.</param>
		Service GetServiceByName (string serviceName)
		{
			if (this.PossibleSiblingTypes.Has (serviceName)) {
				Service newService = this.PossibleSiblingTypes.GetConstructed (serviceName);
				newService.PossibleSiblingTypes = this.PossibleSiblingTypes;

				return newService;
			} else {
				throw new ControlException (ControlException.Cause.BadService, serviceName);
			}
		}

		/// <summary>
		/// Gets the name of the service.
		/// </summary>
		/// <returns>The service name.</returns>
		/// <param name="parameters">Parameters.</param>
		string GetServiceName (IInteraction parameters)
		{
			string serviceName;

			if (parameters.TryGetFallbackString (this.ServiceNameKey, out serviceName)) {				
				return serviceName;
			} else {
				throw new ControlException (ControlException.Cause.NoServiceName, this.ServiceNameKey);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			try {				
				string serviceName = GetServiceName (parameters);
				QuickInteraction successInteraction = new QuickInteraction (parameters);
				successInteraction [this.ServiceIdKey] = GetServiceByName (serviceName).ModelID;

				return this.Successful.TryProcess (successInteraction);
			} catch (ControlException ex) {
				return this.Failure.TryProcess (new FailureInteraction (parameters, ex));
			}
		}
	}
}

