using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Filesystem
{
	public class ShellKill : ShellSignal
	{
		protected override bool Process (IInteraction parameters)
		{
			return this.Shell.TryProcess (new ShellSignalInteraction (
				isKill = true
			));
		}
	}
}

