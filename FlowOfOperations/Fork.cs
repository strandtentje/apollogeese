using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Threading;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Fork : Service
	{
		private bool IsWorking { get; set; }

		Service offloadBranch;
		WaitingQueue<IInteraction> jobs = new WaitingQueue<IInteraction>();
		Thread worker;

		public override string Description {
			get {
				return "Pass interactions into offload thread";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "offload") offloadBranch = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			worker  = new Thread(JobWorker);
			IsWorking = true;
			worker.Start();
		}

		private void JobWorker (object argument)
		{
			while (IsWorking) {
				offloadBranch.TryProcess (jobs.Dequeue ());
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			jobs.Enqueue(parameters);
			return true;
		}
	}
}

