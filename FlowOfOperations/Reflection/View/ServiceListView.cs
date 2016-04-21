using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ServiceList : Service
	{
		public override string Description {
			get {
				return "";
			}
		}

		private Service SiblingIterator = Stub;

		protected override void Initialize (Settings modSettings)
		{
			
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "siblingiterator")
				SiblingIterator = e.NewValue;
			else if (e.Name == "iterator")
				SiblingIterator = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;

			SimpleInteraction foundType;

			foreach (string name in PossibleSiblingTypes.Dictionary.Keys) {
				foundType = new SimpleInteraction (parameters);
				foundType ["servicename"] = name;
				success &= SiblingIterator.TryProcess(foundType);
			}

			return success;
		}
	}
}

