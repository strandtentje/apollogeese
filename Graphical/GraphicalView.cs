using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Graphical
{
	public class GraphicalView : Service
	{
		public override string Description {
			get {
				return "Creates OpenGL Window";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{

		}
	}
}

