using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using MModule = BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module.Module;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ModuleView : FlowView
	{
		protected override Service GetModel (IInteraction parameters)
		{
			object serviceIdObject;

			if (parameters.TryGetFallback ("serviceid", out serviceIdObject)) {
				int serviceIdInt;
				if (int.TryParse (serviceIdObject.ToString (), out serviceIdInt)) {
					Service candidate = ModelLookup [serviceIdInt];
					if (candidate is MModule) {
						MModule module = (MModule)candidate;
						if (module.BranchName != null) {
							return module.GetService (module.BranchName);
						} else {
							throw new Exception ("Module call requires branch too");
						}
					} else {
						throw new Exception ("This service isn't a module");
					}
				} else {
					throw new Exception ("Expected model id");
				}
			} else {
				throw new Exception ("Value of\t modelid not set");
			}
		}
	}
}

