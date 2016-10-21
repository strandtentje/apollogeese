using System;
using SystemProcess = System.Diagnostics.Process;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;
using BorrehSoft.Utensils.Collections.Maps;

namespace Filesystem
{
	public class SystemShell : Service
	{
		public override string Description {
			get {
				return "Generic command executor DON'T USE THIS FUCKER";
			}
		}

		SystemProcess shellProcess;
		Service IncomingLineService;
		string IncomingLineName;

		protected override void Initialize (Settings settings)
		{
			this.IncomingLineName = settings.GetString ("incominglinename", "line");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "line") {
				this.IncomingLineService = e.NewValue;
			}
		}

		public override void Dispose ()
		{
			this.shellProcess.Close ();
		}

		void ShellProcess_OutputDataReceived (object sender, DataReceivedEventArgs e)
		{
			this.IncomingLineService.TryProcess (new SimpleInteraction (
				null, this.IncomingLineName, e.Data
			));
		}

		protected override bool Process (IInteraction parameters)
		{
			var signal = Closest<ShellSignalInteraction>.From (parameters);

			if (signal.IsKill) {
				shellProcess.Dispose ();
			} else {
				shellProcess = SystemProcess.Start (signal.ProcessInfo);
				shellProcess.BeginOutputReadLine ();
				shellProcess.OutputDataReceived += ShellProcess_OutputDataReceived;
			}
		}
	}
}

