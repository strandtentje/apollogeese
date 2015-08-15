using System;
using System.IO;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Indexes the filesystem at a certain root directory
	/// </summary>
	public class FilesystemIndexer : KeywordService
	{
		public override string Description {
			get {
				return this.RootPath;
			}
		}

		/// <summary>
		/// The root path to index.
		/// </summary>
		[Instruction("Path to start indexing")]
		public string RootPath { get; set; }
		Service newFile, deletedFile, newDirectory, deletedDirectory;
		/// <summary>
		/// A cache of filesystem informations
		/// </summary>
		Map<FileSystemInfo> infoCache = new Map<FileSystemInfo>();

		protected override void Initialize (Settings modSettings)
		{
			RootPath = modSettings.GetString("rootpath", ".");
			KeywordSplitterRegex = modSettings.GetString ("keywordsplitterregex", @"\W|_");

			Thread walkPathThread = new Thread(WalkPath);
			walkPathThread.Start(RootPath);
		}

		/// <summary>
		/// Indexs a directory; recursive
		/// </summary>
		/// <param name="path">Path.</param>
        private void IndexDirectory(string path)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string childPath in files)
            {
                try
                {
                    NewItem(new FileInfo(childPath));
                }
                catch (Exception exception)
                {
                    Secretary.Report(5, exception.Message);
                }
            }

            foreach (string childPath in directories)
            {
                try
                {
                    NewItem(new DirectoryInfo(childPath));
                    IndexDirectory(childPath);
                }
                catch (Exception exception)
                {
                    Secretary.Report(5, exception.Message);
                }
            }
        }

		/// <summary>
		/// Helper method to start the directory indexing recursion.
		/// </summary>
		/// <param name="pathObject">Path object.</param>
		private void WalkPath (object pathObject)
		{
			string path = (string)pathObject;

            IndexDirectory(path);            

			FileSystemWatcher futureChanges = new FileSystemWatcher(path);
			futureChanges.Created += NewItemHandler;
			futureChanges.Deleted += RemoveItemHandler;
			futureChanges.Renamed += RenameItemHandler;
		}

		/// <summary>
		/// Handles new items
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void NewItemHandler (object sender, FileSystemEventArgs e)
		{
			FileAttributes fileAttributes = File.GetAttributes(e.FullPath);
			if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
				NewItem(new DirectoryInfo(e.FullPath));
			else 
				NewItem(new FileInfo(e.FullPath));
		}	

		/// <summary>
		/// Handles removed items
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void RemoveItemHandler (object sender, FileSystemEventArgs e)
		{
			if (infoCache.Has(e.FullPath))
				RemoveItem(infoCache[e.FullPath]);
		}

		/// <summary>
		/// Handles renamed items.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void RenameItemHandler (object sender, RenamedEventArgs e)
		{
			RemoveItemHandler(sender, e);
			NewItemHandler(sender, e);
		}
	
		/// <summary>
		/// Removes an item.
		/// </summary>
		/// <param name="info">Info.</param>
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

		/// <summary>
		/// Adds a new item
		/// </summary>
		/// <param name="info">Info.</param>
		void NewItem (FileSystemInfo info)
		{
			try {
				string[] keywords = KeywordSplitter.Split (info.Name.ToLower ());
				IInteraction newInteraction = new FilesystemChangeInteraction (info, keywords, RootPath);

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

