using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
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

		protected override void Initialize (Settings modSettings)
		{

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
					this.Iterator.TryProcess (new SettingInteraction (parameters, pair.Key, pair.Value));
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			object modelIdObject;

			if (parameters.TryGetFallback ("serviceid", out modelIdObject)) {
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

