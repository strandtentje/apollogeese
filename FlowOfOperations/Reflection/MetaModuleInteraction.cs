using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;
using MModule = BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module.Module;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class MetaModuleInteraction : MetaServiceInteraction
	{
		public MetaModuleInteraction (
			IInteraction parentParameters, Service model, Map<object> metaData) : base(parentParameters, model, metaData)
		{
			this ["moduledescription"] = metaData.GetString ("description", "");
			this ["moduledetail"] = metaData.GetString ("detail", "");
		}
	}
}
