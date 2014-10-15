using System;
using System.IO;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Extensions.BasicWeblings.Site.Filesystem;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	public class FilesystemIndexer : Service
	{
		static Regex keywordMatcher = new Regex("[a-z]+|[0-9]+");

		public override string Description {
			get {
				return "Walks the filesystem";
			}
		}

		string rootPath, dirUrl, fileUrl;
		Service newItem, deletedItem;

		protected override void Initialize (Settings modSettings)
		{
			rootPath = modSettings.GetString("rootpath", ".");
			dirUrl = modSettings.GetString("dirurl", "navigate");
			fileUrl = modSettings.GetString("fileurl", "getfile");

			Thread walkPathThread = new Thread(WalkPath);
			walkPathThread.Start(rootPath);
		}

		private void WalkPath (object pathObject)
		{
			string path = (string)pathObject;
			DirectoryInfo rootDirectory = new DirectoryInfo (path);
			FileSystemInfo[] children = rootDirectory.GetFileSystemInfos ("*", SearchOption.AllDirectories);

			foreach (FileSystemInfo directory in children) {
				newItem.TryProcess(new FilesystemChangeInteraction(directory.FullName));
			}

			FileSystemWatcher futureChanges = new FileSystemWatcher(path);
			futureChanges.Created += NewFile;
			futureChanges.Deleted += RemoveFile;
			futureChanges.Renamed += RenameFile;
		}
	
		void RenameFile (object sender, RenamedEventArgs e)
		{
			RemoveFile(sender, e);
			NewFile(sender, e);
		}
	
		void RemoveFile (object sender, FileSystemEventArgs e)
		{
			deletedItem.TryProcess(
				new FilesystemChangeInteraction(e.FullPath));
		}

		void NewFile (object sender, FileSystemEventArgs e)
		{
			newItem.TryProcess(
				new FilesystemChangeInteraction(e.FullPath));
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "new") newItem = e.NewValue;
			if (e.Name == "deletion") deletedItem = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	}
}

