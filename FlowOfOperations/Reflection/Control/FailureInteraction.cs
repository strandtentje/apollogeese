using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class FailureInteraction : QuickInteraction
	{
		public FailureInteraction (IInteraction parent, string initErrorMessage, string initErrorDetail) : base(parent)
		{
			this ["errormessage"] = initErrorMessage;
			this ["errordetail"] = initErrorDetail;
		}

		public FailureInteraction (IInteraction parent, ControlException ex) : base(parent)
		{
			this ["errormessage"] = ex.Message;		
		}
	}
}
