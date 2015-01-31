using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class WorkingDirectory : Service
	{
		private string directory;
		private Service Area;

		public override string Description {
			get {
				return directory;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "area")
				Area = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			directory = modSettings.GetString("directory", ".");
			Directory.SetCurrentDirectory(directory);
		}

		protected override bool Process (IInteraction parameters)
		{
			string previous = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(directory);
			bool success = Area.TryProcess(parameters);
			Directory.SetCurrentDirectory(previous);
			return success;
		}
	}
}

