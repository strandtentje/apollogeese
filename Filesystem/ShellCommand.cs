using System;
using SystemProcess = System.Diagnostics.Process;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;
using BorrehSoft.Utensils.Collections.Maps;

namespace Filesystem
{
	public class ShellCommand : Service
	{
		public override string Description {
			get {
				return "Generic command executor DON'T USE THIS FUCKER";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["command"] = defaultParameter;
		}

		Service IncomingLineService;

		SystemProcess shellProcess;

		string IncomingLineName;

		protected override void Initialize (Settings settings)
		{
			this.IncomingLineName = settings.GetString ("incominglinename", "line");

			ProcessStartInfo processInfo = new ProcessStartInfo (
				                               settings.GetString ("command"), 
				                               settings.GetString ("arguments", "")
			                               ) {
				UseShellExecute = false,
				RedirectStandardOutput = true,

			};

			shellProcess = SystemProcess.Start (processInfo);
			shellProcess.BeginOutputReadLine ();
			shellProcess.OutputDataReceived += ShellProcess_OutputDataReceived;
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
	}
}

