using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class FailureInteraction : QuickInteraction
	{
		public FailureInteraction (string initErrorMessage, string initErrorDetail)
		{
			this ["errormessage"] = initErrorMessage;
			this ["errordetail"] = initErrorDetail;
		}

		public FailureInteraction (ControlException ex)
		{
			this ["errormessage"] = ex.Message;		
		}
	}
}
