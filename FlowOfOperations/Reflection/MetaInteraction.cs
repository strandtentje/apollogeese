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
	class MetaInteraction : SimpleInteraction
	{
		public MetaInteraction (IInteraction parentParameters, 
			Service origin, string branchName, Service target) : base(
				parentParameters)
		{
			this["origin"] = origin.ModelID;
			this["branchname"] = branchName;
			this["target"] = target.ModelID;
		}
	}

}

