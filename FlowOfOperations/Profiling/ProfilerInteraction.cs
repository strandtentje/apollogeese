using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class ProfilerInteraction : SimpleInteraction
	{
		public ProfilerInteraction (
			long totalTimeSpent, long totalMeasurements, 
			Service service, IInteraction parameters) : base (parameters)
		{
			this ["serviceid"] = service.ModelID;
			this ["configline"] = service.ConfigLine;
			this ["description"] = service.Description;	

			this ["totalticks"] = totalTimeSpent;
			this ["totalmeasurements"] = totalMeasurements;

			this ["serviceticks"] = service.Hog.TotalTicksSpent;
			this ["serviceaverage"] = service.Hog.TicksPerMeasurement;
			this ["servicemeasurements"] = service.Hog.MeasurementCount;

			this ["relativeticks"] = (1000 * service.Hog.TotalTicksSpent) / totalTimeSpent;
			this ["relativemeasurements"] = (1000 * service.Hog.MeasurementCount) / totalMeasurements;
		}
	}

}

