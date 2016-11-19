using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	public class ShellStart : ShellSignal
	{
		ProcessStartInfo processInfo;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["command"] = defaultParameter;
		}

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

