using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class IterateStringArray : Service
	{
		public IterateStringArray ()
		{

		}

		public override string Description {
			get {
				return "Repeats interaction for array member";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		List<string> iterateTargets = new List<string>();

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.PreviousValue != null)
				iterateTargets.Remove(e.Name);

			if (e.NewValue != null)
				iterateTargets.Add(e.Name);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			object uncastedEnumerable;

			foreach (string iterateName in iterateTargets) {
				if (parameters.TryGetFallback(iterateName, out uncastedEnumerable))
				{
					IEnumerable<string> enumerable = uncastedEnumerable as IEnumerable<string>;
					Service branch = Branches[iterateName];
					if (enumerable != null) 
						foreach(string iterateValue in enumerable)				
							success &= branch.TryProcess(new IterateInteraction(
								parameters, iterateName, iterateValue));
				}
			}

			return success;
		}
	}
}

