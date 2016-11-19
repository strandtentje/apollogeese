using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Fork : SingleBranchService
	{
		private bool IsWorking { get; set; }

		Queue<IInteraction> jobs = new Queue<IInteraction>();
		Thread worker;

		public override string Description {
			get {
				return "Pass interactions into offload thread";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			worker  = new Thread(JobWorker);
			IsWorking = true;
			worker.Start();
		}

		Semaphore locker = new Semaphore(0,1);

		private void JobWorker (object argument)
		{
			while (IsWorking) {
				if (jobs.Count == 0) {
					locker.WaitOne ();
					locker.Release ();
				}
				lock (jobs) {
					WithBranch.TryProcess (jobs.Dequeue ());
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			lock (jobs) {
				jobs.Enqueue(parameters);
				locker.Release ();
				locker.WaitOne ();
			}

			return true;
		}
	}
}

