using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using BorrehSoft.Utilities.Log;

namespace Filesystem
{
	public abstract class FSService : Service
	{		
		protected DirectoryInfo rootDirectory;

		public string RootPath {
			get {
				return rootDirectory.FullName;
			} set {
				rootDirectory = new DirectoryInfo (value);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["rootpath"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			RootPath = settings.GetString("rootpath", ".");
		}

		protected Service FileFound;
		protected Service DirectoryFound;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "file") {
				FileFound = e.NewValue;
			} else if (e.Name == "directory") {
				DirectoryFound = e.NewValue;
			}
		}

	}
}

