using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections;
using System.Collections.Generic;
using Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Profiler : Service
	{
		Service Iterator;

		int Limit;

		string ConfigurationRoot;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") {
				this.Iterator = e.NewValue;
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["configroot"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			ConfigurationRoot = settings.GetString ("configroot", "");
			Limit = settings.GetInt ("limit", 25);
		}

		public override string Description {
			get {
				return string.Format ("Profiler of all in {0}", ConfigurationRoot);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			SortedList<long, Service> sortedByAvg;
			var comparer = new ReverseDuplicateKeyComparer<long> ();
			sortedByAvg = new  SortedList<long, Service>(comparer);

			long totalTimeSpent = 0, totalMeasurements = 0;

			bool doReset = parameters is ResetInteraction;

			foreach (Service service in Service.ModelLookup.Values) {
				if (service != null) {
					if (doReset) {
						service.Hog.Reset ();
					}
					if ((service.ConfigLine ?? "").StartsWith (ConfigurationRoot)) {
						sortedByAvg.Add (service.Hog.TicksPerMeasurement, service);
						totalTimeSpent += service.Hog.TotalTicksSpent;
						totalMeasurements += service.Hog.MeasurementCount;
					}
				}
			}
		
			int iterated = 0;

			foreach (var keyedService in sortedByAvg) {
				successful = this.Iterator.TryProcess (
					new ProfilerInteraction (
						totalTimeSpent, 
						totalMeasurements, 
						keyedService.Value, 
						parameters
					)
				);

				if (iterated > this.Limit) {
					break; // it down, yo
				}
			}

			return successful;
		}
	}
}

