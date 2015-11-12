using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using BorrehSoft.Utensils.Log;

namespace Filesystem
{
	public class FindFile : FSService
	{
		public override string Description {
			get {
				return string.Format ("find file in {0}", this.RootPath);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			FileSystemInfo[] infos = rootDirectory.GetFileSystemInfos ("*", SearchOption.AllDirectories);
			
			foreach (FileSystemInfo info in infos) {
				try {
					if (info is FileInfo) {
						success &= FileFound.TryProcess (new FileInteraction (info, this.RootPath, parameters));
					} else if (info is DirectoryInfo) {
						success &= DirectoryFound.TryProcess(new DirectoryInteraction(info, this.RootPath, parameters));
					}					
				} catch (Exception ex) {					
					Secretary.Report(5, "Inclusion of new file failed; ", ex.Message);
				}
			}

			return success;
		}
	}
}

