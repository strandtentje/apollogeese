using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	public abstract class ShellSignal : Service
	{
		public override string Description {
			get {
				return "Provid signal into next best SystemShell";
			}
		}

		protected Service Shell;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "shell") {
				this.Shell = e.NewValue;
			}
		}
	}
}

