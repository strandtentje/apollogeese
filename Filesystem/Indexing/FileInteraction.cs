using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using BorrehSoft.ApolloGeese.Extensions.Filesystem;

namespace Filesystem
{
	class FileInteraction : FSInteraction
	{
		public FileInteraction (
			FileSystemInfo info, string rootpath, IInteraction parameters = null) : base(
			info, rootpath, parameters)
		{

		}

		public FileInfo GetInfo() {
			return (FileInfo)Info;
		}

		public long ByteCount {
			get {
				return GetInfo().Length;
			}
		}

		public string FileSize {
			get {
				return FilesystemItemInteraction.Filesize (ByteCount);
			}
		}

		public string ShortName {
			get {
				return GetInfo().Name.Remove (GetInfo().Name.Length - GetInfo().Extension.Length);
			}
		}

		public string Extension {
			get {
				return GetInfo ().Extension.TrimStart ('.').ToLower ();
			}
		}
	}

}

