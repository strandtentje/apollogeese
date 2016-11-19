using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class SettingsView : Service
	{
		public override string Description {
			get {
				return "service configuration viewer";
			}
		}

		Settings sourceSettings = null;
		string sourceSettingsVariable = null;

		protected override void Initialize (Settings modSettings)
		{
			sourceSettings = null;
			sourceSettingsVariable = null;

			if (modSettings.Has ("sourcesettings")) {
				this.sourceSettings = modSettings.GetSubsettings ("sourcesettings");
			} else if (modSettings.Has ("sourcesettingsvariable")) {
				this.sourceSettingsVariable = modSettings.GetString ("sourcesettingsvariable");
			}
		}

		Service None { get; set; }
		Service Single { get; set; }
		Service Iterator { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "none")
				this.None = e.NewValue;
			if (e.Name == "iterator")
				this.Iterator = e.NewValue;
		}

		void ViewMap (Map<object> data, IInteraction parameters)
		{
			if (data.Dictionary.Count == 0) {
				this.None.TryProcess (parameters);
			} else {
				foreach (KeyValuePair<string, object> pair in data.Dictionary) {
					if (!pair.Key.StartsWith ("_")) {
						this.Iterator.TryProcess (new SettingInteraction (parameters, pair.Key, pair.Value));
					}
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			object modelIdObject;

			if (sourceSettings != null) {
				ViewMap (sourceSettings, parameters);
			} else if (sourceSettingsVariable != null) {
				object contextualSourceSettings;
				if (parameters.TryGetFallback (this.sourceSettingsVariable, out contextualSourceSettings)) {
					if (contextualSourceSettings is Map<object>) {
						ViewMap ((Map<object>)contextualSourceSettings, parameters);
					} else {
						throw new Exception (string.Format ("Expected map in context at {0}", this.sourceSettingsVariable));
					}
				} else {
					throw new Exception (string.Format ("Expected something at {0}", this.sourceSettingsVariable));
				}
			} else if (parameters.TryGetFallback ("serviceid", out modelIdObject)) {
				int modelIdInt;
				if (int.TryParse (modelIdObject.ToString (), out modelIdInt)) {
					Service candidate = ModelLookup [modelIdInt];

					ViewMap (candidate.GetSettings (), parameters);
				} else {
					throw new Exception ("Expected model id");
				}
			} else {
				throw new Exception ("Value of\t modelid not set");
			}

			return true;
		}
	}
}

