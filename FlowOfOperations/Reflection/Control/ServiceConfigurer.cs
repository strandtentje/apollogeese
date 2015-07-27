using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ServiceConfigurer : ServiceMutator
	{
		public override string Description {
			get {
				return "Configure service";
			}
		}

		string ServiceIdKey;

		protected override void Initialize (Settings modSettings)
		{
			this.ServiceIdKey = modSettings.GetString ("serviceidkey", "serviceid");
		}

		/// <summary>
		/// Gets the nearest interaction that is also a map, from context.
		/// </summary>
		/// <returns>The context settings.</returns>
		/// <param name="parameters">Parameters.</param>
		Settings GetContextSettings (IInteraction parameters)
		{
			IInteraction closestMapInteraction;

			if (parameters.TryGetClosest (typeof(Map<object>), out closestMapInteraction)) {
				// this cast is wrong. so, so wrong.
				Map<object> closestMap = (Map<object>)closestMapInteraction;
				// this construction is so right, however.
				return new Settings (closestMap);
			} else {
				throw new ControlException (ControlException.Cause.NoMapFound, "");
			}
		}


		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;

			try {
				int serviceInt = GetServiceInt (parameters, this.ServiceIdKey);
				Service foundService = GetServiceById(serviceInt);
				Settings config = GetContextSettings(parameters);
				
				if (!foundService.SetSettings (config)) {
					successful &= Failure.TryProcess(new FailureInteraction(foundService.InitErrorMessage, foundService.InitErrorDetail));
				} else {
					successful &= Successful.TryProcess(new MetaServiceInteraction(parameters, foundService));
				}
			} catch(ControlException ex) {
				successful &= Failure.TryProcess (new FailureInteraction (ex));
			}

			return successful;
		}
	}
}

