using System;
using System.IO;
using System.Threading;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class AsyncRunner : Service
    {
		public override string Description => "Runs async";

		public Service AsyncBranch { get; private set; }
		public Service AwaitBranch { get; private set; }
		public string ContentType { get; private set; }

		protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "async") {
				this.AsyncBranch = e.NewValue;
			} else if (e.Name == "await") {
				this.AwaitBranch = e.NewValue;
			}
		}

		protected override void Initialize(Settings settings)
		{
			this.ContentType = Settings.GetString("contenttype", "text/plain");
			base.Initialize(settings);
		}

		protected override bool Process(IInteraction parameters)
		{
			Thread thread = new Thread(runner);
			thread.Start(parameters);
			return true;
		}

		private void runner(object obj)
		{
			var parameters = obj as IInteraction;
			var createdData = new MemoryStream();
			var asyncParameters = new SimpleOutgoingInteraction(createdData, parameters);
			AsyncBranch.TryProcess(asyncParameters);

			asyncParameters.Done();
			createdData.Position = 0;
			var resultParameters = new SimpleIncomingInteraction(createdData, parameters, "asyncrunner", this.ContentType);
			AwaitBranch.TryProcess(resultParameters);
		}
	}
}
