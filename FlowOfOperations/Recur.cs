using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Timers;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Recur : SingleBranchService
	{
		public override string Description {
			get {
				return "Buffer between incoming and outgoing";
			}
		}

		public override void LoadDefaultParameters (object defaultParameter)
		{
			Settings ["timeout"] = (int)defaultParameter;
		}

		Timer timeout;

		public int TimeOut { 
			get { return (int)timeout.Interval; }
			private set {
				if (timeout != null)
					timeout.Dispose ();
				timeout = new Timer (value);
				timeout.AutoReset = true;
				timeout.Elapsed += HandleElapsed;
				timeout.Start ();
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.TimeOut = settings.GetInt ("timeout", 1000);

			base.Initialize (settings);
		}

		Queue<IInteraction> items = new Queue<IInteraction>();
				
		void HandleElapsed (object sender, ElapsedEventArgs e)
		{
			lock (items)
				while (items.Count > 0) 
					this.WithBranch.TryProcess (items.Dequeue ());
		}

		protected override bool Process (IInteraction parameters)
		{
			lock (items)
				items.Enqueue (parameters);

			return true;
		}
	}
}

