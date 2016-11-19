using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;

namespace Filesystem
{
	class DirectoryInteraction  : FSInteraction
	{
		public DirectoryInteraction (
			FileSystemInfo info, string rootPath, IInteraction parameters = null) : base(
			info, rootPath, parameters)
		{

		}
	}
}

