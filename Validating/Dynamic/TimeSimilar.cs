using System;
using Validating;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Dynamic
{
	public class TimeSimilar : Equals
	{
		TimeSpan Tollerance;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Tollerance = TimeSpan.Parse(settings.GetString("tollerance", "00:05"));
		}

		protected override Service Compare (object fromValue, object toValue)
		{
			DateTime fromDate, toDate;
			if (fromValue is DateTime) fromDate = (DateTime)fromValue;
			else if (!DateTime.TryParse(fromValue.ToString(), out fromDate)) return Failure;

			if (toValue is DateTime) toDate = (DateTime)toValue;
			else if (!DateTime.TryParse(toValue.ToString(), out toDate)) return Failure;

			if (toDate == fromDate) return Successful;
			else if ((fromDate > toDate) && ((fromDate - toDate) < this.Tollerance)) return Successful;
			else if ((toDate > fromDate) && ((toDate - fromDate) < this.Tollerance)) return Successful;
			else return Failure;
		}
	}
}

