using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	class ShellSignalInteraction
	{
		public ProcessStartInfo ProcessInfo { get; private set; }
		public bool IsKill { get; private set; }

		public ShellSignalInteraction (ProcessStartInfo processInfo)
		{
			this.ProcessInfo = processInfo;
		}

		public ShellSignalInteraction (bool isKill)
		{
			this.IsKill = isKill;
		}
	}

}

