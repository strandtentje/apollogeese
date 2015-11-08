using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using CronNET;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Schedule : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format ("Triggers on {0}", job.ToString());
			}
		}

		public static CronDaemon daemon = new CronDaemon();

		public CronJob job = null;

		[Instruction("Crontab line")]
		public string Crontab {
			get { 
				if (job == null) {
					return "";
				} else {
					return job.Schedule;
				}
			}
			set {
				if (job != null) {
					job.abort ();
				}

				job = new CronJob (value, Start);
				daemon.AddJob (job);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["crontab"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			Crontab = settings.GetString ("crontab");
			daemon.Start ();
		}

		void Start ()
		{
			Successful.TryProcess (new SimpleInteraction (null, "crontab", Crontab));
		}

		protected override bool Process (IInteraction parameters)
		{
			return this.Successful.TryProcess (parameters);
		}
	}
}

