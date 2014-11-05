using System;
using System.IO;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Extensions.BasicWeblings.Site.Filesystem;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	public class FilesystemIndexer : Service
	{
		public override string Description {
			get {
				return "Walks the filesystem";
			}
		}

		string rootPath;
		Service newFile, deletedFile, newDirectory, deletedDirectory;
		Map<FileSystemInfo> infoCache = new Map<FileSystemInfo>();
		Regex KeywordSplitter;

		protected override void Initialize (Settings modSettings)
		{
			rootPath = modSettings.GetString("rootpath", ".");
			KeywordSplitter = new Regex(modSettings.GetString("keywordsplitregex", @"\W|_"));

			Thread walkPathThread = new Thread(WalkPath);
			walkPathThread.Start(rootPath);
		}

		private void WalkPath (object pathObject)
		{
			string path = (string)pathObject;

			DirectoryInfo rootDirectory = new DirectoryInfo (path);
			FileSystemInfo[] children = rootDirectory.GetFileSystemInfos ("*", SearchOption.AllDirectories);

			foreach (FileSystemInfo child in children) {
				NewItem(child);
			}

			FileSystemWatcher futureChanges = new FileSystemWatcher(path);
			futureChanges.Created += NewItemHandler;
			futureChanges.Deleted += RemoveItemHandler;
			futureChanges.Renamed += RenameItemHandler;
		}

		void NewItemHandler (object sender, FileSystemEventArgs e)
		{
			FileAttributes fileAttributes = File.GetAttributes(e.FullPath);
			if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
				NewItem(new DirectoryInfo(e.FullPath));
			else 
				NewItem(new FileInfo(e.FullPath));
		}	

		void RemoveItemHandler (object sender, FileSystemEventArgs e)
		{
			if (infoCache.Has(e.FullPath))
				RemoveItem(infoCache[e.FullPath]);
		}

		void RenameItemHandler (object sender, RenamedEventArgs e)
		{
			RemoveItemHandler(sender, e);
			NewItemHandler(sender, e);
		}
	
		void RemoveItem (FileSystemInfo info)
		{	
			try {
				infoCache.Dictionary.Remove (info.FullName);

				string[] keywords = KeywordSplitter.Split (info.Name.ToLower ());
				IInteraction removalInteraction = new FilesystemChangeInteraction (info, keywords);

				if (info is FileInfo)
					deletedFile.TryProcess (removalInteraction);
				if (info is DirectoryInfo)
					deletedDirectory.TryProcess (removalInteraction);
			} catch(Exception exception) {
				Secretary.Report(5, "Removal of missing file failed; ", exception.Message);
			}
		}

		void NewItem (FileSystemInfo info)
		{
			try {
				string[] keywords = KeywordSplitter.Split (info.Name.ToLower ());
				IInteraction newInteraction = new FilesystemChangeInteraction (info, keywords, rootPath);

				if (info is FileInfo)
					newFile.TryProcess (newInteraction);
				if (info is DirectoryInfo)
					newDirectory.TryProcess (newInteraction);

				infoCache [info.FullName] = info;
			} catch (Exception exception) {
				Secretary.Report(5, "Inclusion of new file failed; ", exception.Message);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "newfile") newFile = e.NewValue;
			if (e.Name == "deletedfile") deletedFile = e.NewValue;
			if (e.Name == "newdirectory") newDirectory = e.NewValue;
			if (e.Name == "deleteddirectory") deletedDirectory = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	}
}

