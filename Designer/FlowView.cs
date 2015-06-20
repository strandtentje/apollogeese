using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Designer
{
	public class FlowView : Service
	{
		public override string Description {
			get {
				return "Program Flow Viewer";
			}
		}

		private Service 
			Model, BlockView,
			InteractionView, SiblingIterator,
			SettingIterator;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "model") Model = e.NewValue;
			if (e.Name == "block") BlockView = e.NewValue;
			if (e.Name == "interaction") InteractionView = e.NewValue;
			if (e.Name == "siblingiterator") SiblingIterator = e.NewValue;
			if (e.Name == "settingiterator") SettingIterator = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches["model"] = Stub; Branches["block"] = Stub;
			Branches["interaction"] = Stub; Branches["siblingiterator"] = Stub;
			Branches["settingiterator"] = Stub;
		}

		bool VisualizeInteraction (Service origin, string branchName, Service target, IInteraction parentParameters)
		{
			QuickInteraction InterContext = new QuickInteraction(parentParameters);

			InterContext["origin"] = origin.ModelID;
			InterContext["branchname"] = branchName;
			InterContext["target"] = target.ModelID;

			return InteractionView.TryProcess(InterContext);
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

				QuickInteraction ModelContext = new QuickInteraction (parentParameters);

				ModelContext ["id"] = model.ModelID;
				ModelContext ["type"] = model.GetType ().Name;
				ModelContext ["description"] =  HttpUtility.HtmlEncode(model.Description);

				foreach (KeyValuePair<string, Service> BranchTuple in model.Branches.Dictionary) {
					success &= VisualizeBlock (BranchTuple.Value, parentParameters, history);
				}

				success &= BlockView.TryProcess (ModelContext);
				
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

			return VisualizeBlock(Model, finishedSiblings);
		}
	}
}

