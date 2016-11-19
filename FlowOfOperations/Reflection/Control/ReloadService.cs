using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ReloadService : ServiceMutator
	{
		public override string Description {
			get {
				return "Configure service";
			}
		}

		[Instruction("Name of variable in context where id of target service is stored.", "serviceid")]
		public string ServiceIdKey { get; set; }

		[Instruction("When set to true, new config replaces old one. Otherwise, append.")]
		public bool Replace { get; set; }

		[Instruction("When set to true, new config will be loaded upon reload. Otherwise, only service will be reloaded.")]
		public bool Reconfigure { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			this.ServiceIdKey = modSettings.GetString ("serviceidkey", "serviceid");
			this.Reconfigure = modSettings.GetBool ("reconfigure", false);
			this.Replace = modSettings.GetBool ("replace", false);
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
				Settings config = foundService.GetSettings();

				if (Reconfigure) {
					config = GetContextSettings(parameters);

					if (!Replace) {
						config = Settings.FromMerge(config, foundService.GetSettings());
					}
				}
				
				if (!foundService.SetSettings (config)) {
					successful &= Failure.TryProcess(new FailureInteraction(parameters, foundService.InitErrorMessage, foundService.InitErrorDetail));
				} else {
					successful &= Successful.TryProcess(MetaServiceInteraction.FromService(parameters, foundService));
				}
			} catch(ControlException ex) {
				successful &= Failure.TryProcess (new FailureInteraction (parameters, ex));
			}

			return successful;
		}
	}
}

