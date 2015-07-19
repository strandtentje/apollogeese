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
			InteractionView, SiblingIterator,
			SettingIterator;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "block") BlockView = e.NewValue;
			if (e.Name == "module")	ModuleView = e.NewValue;
			if (e.Name == "interaction") InteractionView = e.NewValue;
			if (e.Name == "siblingiterator") SiblingIterator = e.NewValue;
			if (e.Name == "settingiterator") SettingIterator = e.NewValue;
		}

		protected virtual Service GetModel(IInteraction parameters) {
			return Branches ["model"];
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches["model"] = Stub; 
			Branches ["module"] = Stub;
			Branches["block"] = Stub;
			Branches["interaction"] = Stub; 
			Branches["siblingiterator"] = Stub;
			Branches["settingiterator"] = Stub;
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

			if (!history.Contains (model.ModelID)) {
				history.Add(model.ModelID);

				MetaServiceInteraction modelContext = new MetaServiceInteraction (parentParameters, model);

				foreach (KeyValuePair<string, Service> BranchTuple in model.Branches.Dictionary) {
					success &= VisualizeBlock (BranchTuple.Value, parentParameters, history);
				}

				if ((model is MModule) && (ModuleView != Stub))
					success &= ModuleView.TryProcess (modelContext);
				else
					success &= BlockView.TryProcess (modelContext);
				
				foreach (KeyValuePair<string, Service> BranchTuple in model.Branches.Dictionary) {
					success &= VisualizeInteraction (model, BranchTuple.Key, BranchTuple.Value, parentParameters);
				}
			}

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			OutgoingIterator siblings = new OutgoingIterator(parameters, "siblings");

			foreach (string name in PossibleSiblingTypes.Dictionary.Keys) {
				siblings["typename"] = name;
				success &= SiblingIterator.TryProcess(siblings);
			}

			QuickInteraction finishedSiblings = new QuickInteraction (parameters);
			finishedSiblings ["siblings"] = siblings.GetFinished ();

			return VisualizeBlock(GetModel(parameters), finishedSiblings);
		}
	}
}

