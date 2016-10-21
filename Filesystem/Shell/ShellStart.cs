using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	public class ShellStart : ShellSignal
	{
		ProcessStartInfo processInfo;

		protected override void Initialize (Settings settings)
		{
			processInfo = new ProcessStartInfo (
				settings.GetString ("command"), 
				settings.GetString ("arguments", "")
			) {
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};
		}

		protected override bool Process (IInteraction parameters)
		{
			return this.Shell.TryProcess (new ShellSignalInteraction (
				parameters,
				this.processInfo
			));
		}
	}
}

