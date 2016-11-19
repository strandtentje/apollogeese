using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class FormatDateTime : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format (
					"Format date time from variable {0} to format '{1}'",
					this.VariableName,
					this.DateTimeFormat);
			}
		}

		[Instruction("Name of variable to source datetime from", "datetime")]
		public string VariableName { get; set; }

		[Instruction("Format of date-time", "d")]
		public string DateTimeFormat { get; set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] variableAndFormat = defaultParameter.Split ('|');

			if (variableAndFormat.Length > 0) {
				this.Settings["variablename"] = variableAndFormat [0];
				if (variableAndFormat.Length > 1) {
					this.Settings["format"] = variableAndFormat [1];
				}
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.VariableName = settings.GetString ("variablename", "datetime");
			this.DateTimeFormat = settings.GetString ("format", "d");			
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			object dateTimeCandidate;
			IInteraction targetOutgoing;

			if (parameters.TryGetFallback (this.VariableName, out dateTimeCandidate) && dateTimeCandidate is DateTime) {
				if (parameters.TryGetClosest (typeof(IOutgoingBodiedInteraction), out targetOutgoing)) {
					IOutgoingBodiedInteraction target = (IOutgoingBodiedInteraction)targetOutgoing;
					DateTime source = (DateTime)dateTimeCandidate;

					byte[] formatted = target.Encoding.GetBytes (source.ToString (this.DateTimeFormat));
					target.OutgoingBody.Write (formatted, 0, formatted.Length);

					success &= (Successful == null) || Successful.TryProcess (parameters);
				} else {
					success &= (Failure == null) || Failure.TryProcess (parameters);
				}
			} else {
				string actualValueType;

				if (dateTimeCandidate == null) {
					actualValueType = "NULL";
				} else {
					actualValueType = dateTimeCandidate.GetType ().ToString ();
				}

				success &= (Failure == null) || Failure.TryProcess (parameters);
			}

			return success;
		}
	}
}

