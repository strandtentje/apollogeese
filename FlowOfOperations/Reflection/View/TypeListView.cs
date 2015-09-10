using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class TypeList : Service
	{
		public override string Description {
			get {
				return "";
			}
		}

		private Service SiblingIterator;

		protected override void Initialize (Settings modSettings)
		{
			Branches["siblingiterator"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "siblingiterator") SiblingIterator = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;

			SimpleInteraction foundType;

			foreach (string name in PossibleSiblingTypes.Dictionary.Keys) {
				foundType = new SimpleInteraction (parameters);
				foundType ["typename"] = name;
				success &= SiblingIterator.TryProcess(foundType);
			}

			return success;
		}
	}
}

