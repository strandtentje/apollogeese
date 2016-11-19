using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	class ShellSignalInteraction : SimpleInteraction
	{
		public ProcessStartInfo ProcessInfo { get; private set; }
		public bool IsKill { get; private set; }

		public ShellSignalInteraction (
			IInteraction parameters, 
			ProcessStartInfo processInfo
		) : base(parameters) {
			this.ProcessInfo = processInfo;
		}

		public ShellSignalInteraction (bool isKill)
		{
			this.IsKill = isKill;
		}
	}

}

