using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class MetaServiceInteraction : QuickInteraction
	{
		public MetaServiceInteraction (IInteraction parentParameters, Service model) : base(parentParameters)
		{
			this ["serviceid"] = model.ModelID;
			this ["servicetype"] = model.GetType ().Name;
			this ["servicedescription"] =  HttpUtility.HtmlEncode(model.Description);
		}
	}

}

