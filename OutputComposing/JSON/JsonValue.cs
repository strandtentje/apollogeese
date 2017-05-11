using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Settings;
using Commons.Json;
using System.Web;
using BorrehSoft.Utilities.Log;
using Interactions;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class JsonValue : Service
	{
		const string OverrideTail = "_override";
		const string SingleValueName = "singlevalue";
		/// <summary>
		/// If present, return only JSON format of single value,
		/// instead of entire object.
		/// </summary>
		string SingleValue;
		/// <summary>
		/// The valuename remappings ie. T1<-T2
		/// </summary>
		List<Tuple<string, string>> ValueNameRemappings = new List<Tuple<string, string>> ();
		List<Tuple<string, object>> ValueAssignments = new List<Tuple<string, object>> ();

		public override string Description {
			get {
				return ":";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings [SingleValueName] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.SingleValue = settings.GetString (SingleValueName, "");

			foreach (var overridePair in settings.Dictionary) {
				if (overridePair.Key.EndsWith (OverrideTail)) {
					ValueNameRemappings.Add (
						new Tuple<string, string> (
							overridePair.Value.ToString (),
							overridePair.Key.Remove (
								overridePair.Key.Length -
								OverrideTail.Length
							)
						)
					);
				} else {
					ValueAssignments.Add (
						new Tuple<string, object> (
							overridePair.Key,
							overridePair.Value
						)
					);
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			var outgoing = Closest<IOutgoingBodiedInteraction>.From (parameters);

			if (outgoing is IHttpInteraction) {
				try {
					((IHttpInteraction)outgoing).SetContentType ("application/json");
				} catch (HttpException ex) {
					Secretary.Report (5, "Failed setting json mimetype.");
				}
			}

			object value;
			if (SingleValueName == "") {
				JsonObject values = new JsonObject ();
				foreach (var assignment in ValueAssignments) {
					values [assignment.Item1] = new JsonPrimitive (assignment.Item2);
				}
				foreach (var remapping in ValueNameRemappings) {
					if (parameters.TryGetFallback (
						    remapping.Item2, out value)) {
						values [remapping.Item1] = JsonPrimitive.From (value);
					}
				}
				Writer.Into (outgoing).Append (values);
			} else if (parameters.TryGetFallback (SingleValue, out value)) {
				Writer.Into (outgoing).Append (JsonPrimitive.From (value));
			}

			return true;
		}
	}
}

