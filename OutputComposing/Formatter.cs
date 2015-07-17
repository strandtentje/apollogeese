using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Formats text produced downstream. Might be notoriously slow.
	/// </summary>
	public class Formatter : Service
	{
		public override string Description {
			get {
				return "Downstream text formatter";
			}
		}

		private Service begin;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin")
				begin = e.NewValue;
		}

		List<Format> formats = new List<Format>();

		protected override void Initialize (Settings modSettings)
		{
			IEnumerable<object> pairs = (IEnumerable<object>)modSettings.Get ("pairs");

			foreach (object pairObject in pairs) {
				Settings pair = (Settings)pairObject;
				if (pair.GetBool ("htmlentityescape", false)) {
					formats.Add (new Format (true));
				} else {
					formats.Add (new Format ((string)pair ["pattern"], (string)pair ["format"]));
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			if (parameters is INosyInteraction) {
				return begin.TryProcess (parameters);
			} else {
				IOutgoingBodiedInteraction upstreamTarget;
				QuickOutgoingInteraction downstreamTarget;
				MemoryStream formattables;
				bool success;
				string data;

				upstreamTarget = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));
				downstreamTarget = new QuickOutgoingInteraction (formattables = new MemoryStream (), parameters);

				success = begin.TryProcess (downstreamTarget);
				downstreamTarget.Done ();

				formattables.Position = 0;

				using (StreamReader reader = new StreamReader(formattables)) 
					data = reader.ReadToEnd ();
			
				foreach (Format format in formats)
					data = format.Apply (data);

				upstreamTarget.GetOutgoingBodyWriter ().Write (data);
				return success;
			}
		}
	}
}

