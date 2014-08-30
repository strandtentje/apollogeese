using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace GeeseUI
{
	public class EditorNode : Service
	{
		public EditorNode ()
		{

		}

		public override string Description {
			get {
				return "UI to configure connected nodes";
			}
		}

		Service target;
		Structure structure = new Structure();

		protected override void Initialize (Settings modSettings)
		{
			StructureView.Start(structure);
		}

		protected override bool Process (IInteraction parameters)
		{
			return true;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "configurable") {
				structure.SetStartingPoint(e.NewValue);
			}
		}
	}
}

