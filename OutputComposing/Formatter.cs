using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Formats text produced downstream. Might be notoriously slow.
	/// </summary>
	public class Formatter : SingleBranchService
	{
		public override string Description {
			get {
				return "Downstream text formatter";
			}
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
				return WithBranch.TryProcess (parameters);
			} else {
				IOutgoingBodiedInteraction upstreamTarget;
				SimpleOutgoingInteraction downstreamTarget;
				MemoryStream formattables;
				bool success;
				string text;

				upstreamTarget = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));
				downstreamTarget = new SimpleOutgoingInteraction (
					formattables = new MemoryStream (), upstreamTarget.Encoding, parameters);

				success = WithBranch.TryProcess (downstreamTarget);
				downstreamTarget.Done ();

				formattables.Position = 0;

				using (StreamReader reader = new StreamReader(formattables)) 
					text = reader.ReadToEnd ();
			
				foreach (Format format in formats)
					text = format.Apply (text);
				

				byte[] data = Encoding.Unicode.GetBytes (text);

				upstreamTarget.OutgoingBody.Write(data, 0, data.Length);

				return success;
			}
		}
	}
}

