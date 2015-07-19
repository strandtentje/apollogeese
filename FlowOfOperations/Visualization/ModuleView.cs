using System;
using BorrehSoft.ApolloGeese.Duckling;
using MModule = BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module.Module;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ModuleView : FlowView
	{
		protected override Service GetModel (IInteraction parameters)
		{
			object modelIdObject;

			if (parameters.TryGetFallback ("modelid", out modelIdObject)) {
				int modelIdInt;
				if (int.TryParse (modelIdObject.ToString (), out modelIdInt)) {
					Service candidate = ModelLookup [modelIdInt];
					if (candidate is MModule) {
						MModule module = (MModule)candidate;
						if (module.BranchName != null) {
							return module.ModuleBranches [module.BranchName];
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

