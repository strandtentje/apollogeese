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
	public class FlowView : Service
	{
		public override string Description {
			get {
				return "Program Flow Viewer";
			}
		}

		private Service BlockView, ModuleView,
			InteractionView,
			SettingIterator;

		Service StubView;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "block") BlockView = e.NewValue;
			if (e.Name == "module")	ModuleView = e.NewValue;
			if (e.Name == "interaction") InteractionView = e.NewValue;
			if (e.Name == "stub") StubView = e.NewValue;
		}

		protected virtual Service GetModel(IInteraction parameters) {
			return Branches ["model"];
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches["model"] = Stub; 
			Branches["block"] = Stub;
			Branches["module"] = Stub;
			Branches ["stub"] = Stub;
			Branches["interaction"] = Stub; 
		}

		bool VisualizeInteraction (Service origin, string branchName, Service target, IInteraction parentParameters)
		{
			MetaInteraction aboutInteraction = new MetaInteraction (parentParameters,
				                                   origin, branchName, target);

			return InteractionView.TryProcess(aboutInteraction);
		}

		bool VisualizeBlock (Service model, IInteraction parentParameters)
		{
			return VisualizeBlock(model, parentParameters, new List<int> ());
		}

		bool VisualizeBlock (Service model, IInteraction parentParameters, List<int> history)
		{
			bool success = true;

			if (model is Proceed) {
				Service nextModel = ((Proceed)model).GetReturnService (parentParameters, Stub);
				success &= VisualizeBlock(nextModel, parentParameters);
			} else if (!history.Contains (model.ModelID)) {
				history.Add(model.ModelID);


				foreach (KeyValuePair<string, Service> BranchTuple in model.Branches.Dictionary) {
					success &= VisualizeBlock (BranchTuple.Value, parentParameters, history);
				}

				if ((model is MModule) && (ModuleView != Stub)) {
					MetaModuleInteraction modelContext = new MetaModuleInteraction (
						parentParameters, model, ((MModule)model).ModuleMetadata);

					success &= ModuleView.TryProcess (modelContext);
				} else {
					MetaServiceInteraction modelContext = MetaServiceInteraction.FromService(
						parentParameters, model);

					if ((model is StubService) && (StubView != Stub))
						success &= StubView.TryProcess (modelContext);
					else
						success &= BlockView.TryProcess (modelContext);
				}
				
				foreach (KeyValuePair<string, Service> BranchTuple in model.Branches.Dictionary) {
					success &= VisualizeInteraction (model, BranchTuple.Key, BranchTuple.Value, parentParameters);
				}
			}

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			return VisualizeBlock(GetModel(parameters), parameters);
		}
	}
}
