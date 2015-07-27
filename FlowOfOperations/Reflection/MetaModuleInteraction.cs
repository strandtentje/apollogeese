using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;
using MModule = BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module.Module;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class MetaModuleInteraction : QuickInteraction
	{
		public MetaModuleInteraction (
			IInteraction parentParameters, Service model, Map<object> metaData) : base(
			parentParameters, metaData)
		{
			this ["serviceid"] = model.ModelID;
			this ["servicetype"] = model.GetType ().Name;
			this ["servicedescription"] = HttpUtility.HtmlEncode (model.Description);
			this ["moduledescription"] = metaData.GetString ("description", "");
			this ["moduledetail"] = metaData.GetString ("detail", "");
		}
	}
}
