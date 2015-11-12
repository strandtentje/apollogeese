using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;
using System.Collections.Generic;

namespace Filesystem
{
	public class WatchDirectory : FSService
	{
		public override string Description {
			get {
				return string.Format ("watch {0} for changes", this.RootPath);
			}
		}

		Service Gone {
			get;
			set;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "gone") {
				Gone = e.NewValue;
			}
		}

		Queue<string> Changes = new Queue<string> ();

		void HandlePath (string fullPath)
		{			
			Changes.Enqueue (fullPath);
		}

		void HandleChanged (object sender, FileSystemEventArgs e)
		{		
			HandlePath (e.FullPath);
		}

		void HandleRenamed (object sender, RenamedEventArgs e)
		{
			HandlePath (e.OldFullPath);
			HandlePath (e.FullPath);
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			
			FileSystemWatcher futureChanges = new FileSystemWatcher(this.RootPath);
			futureChanges.Changed += HandleChanged;
			futureChanges.Created += HandleChanged;
			futureChanges.Deleted += HandleChanged;
			futureChanges.Renamed += HandleRenamed;
		}

		protected override bool Process (IInteraction parameters)
		{
			string fullPath = "";
			bool success = true;

			while (Changes.Count > 0) {
				fullPath = Changes.Dequeue ();
				
				if (Directory.Exists (fullPath)) {
					success &= DirectoryFound.TryProcess (new DirectoryInteraction (new DirectoryInfo (fullPath), RootPath, parameters));
				} else if (File.Exists (fullPath)) {
					success &= FileFound.TryProcess (new FileInteraction (new FileInfo (fullPath), RootPath, parameters));
				} else {
					success &= Gone.TryProcess (new FSInteraction (new FileInfo (fullPath), RootPath, parameters));
				}
			}

			return success;
		}
	}
}

