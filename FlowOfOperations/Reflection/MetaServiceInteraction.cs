using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class MetaServiceInteraction : SimpleInteraction
	{
		public MetaServiceInteraction (IInteraction parentParameters, Service model, Map<object> metaData) : base(parentParameters, metaData)
		{
			this ["serviceid"] = model.ModelID;
			this ["servicetype"] = model.GetType ().Name;
			this ["servicedescription"] =  HttpUtility.HtmlEncode(model.Description);

			bool isloaded = (model.GetSettings () ?? new Settings()).IsLoaded;
			bool isinitfail = (model.InitErrorMessage ?? "").Length > 0;

			this ["loadstate"] = string.Format ("{0} {1}", 
			                                   (isloaded ? "unaltered" : "altered"),
			                                   (isinitfail ? "uninit" : "init"));

			this ["initfault"] = model.InitErrorMessage;
			this ["initdetail"] = model.InitErrorDetail;
		}

		public static MetaServiceInteraction FromService(IInteraction parentParameters, Service model) {
			return new MetaServiceInteraction(parentParameters, model, new Map<object>());
		}
	}

}

