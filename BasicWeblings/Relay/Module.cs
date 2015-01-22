using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Duckling.Loader;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class Module : Service
	{
		public override string Description {
			get {
				return string.Format("module:{0}:{1}", file, module);
			}
		}

		string file, module;

		protected override void Initialize (Settings modSettings)
		{
			file = (string)modSettings ["file"];
			module = (string)modSettings ["branch"];
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			return InstanceLoader.GetInstances (file) [module].TryProcess (parameters);
		}
	}
}

