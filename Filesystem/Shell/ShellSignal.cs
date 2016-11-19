using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Diagnostics;

namespace Filesystem
{
	public abstract class ShellSignal : SingleBranchService
	{
		public override string Description {
			get {
				return "Provid signal into next best SystemShell";
			}
		}

		protected Service Shell { get { return this.WithBranch; } }
	}
}

