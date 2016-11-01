using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	public class Spliterate : SplitterService
	{
		[Instruction("Source variable")]
		public string From { get; private set; }

		[Instruction("Target variable")]
		public string To { get; private set; }

		public override string Description {
			get {
				return string.Format ("Iterator over {0} into {1}", this.From, this.To);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] fromTo = defaultParameter.Split ('>');

			if (fromTo.Length == 2) {
				Settings ["from"] = fromTo [0];
				Settings ["to"] = fromTo [1];
			}
		}

		protected override void Initialize (Settings settings)
		{
			From = settings.GetString ("from");
			To = settings.GetString ("to");
			SplitterRegex = settings.GetString ("splitter", WordSplitter);
		}

		IntMap<Service> numBranches = new IntMap<Service> ();

		IntMap<Service> countBranches = new IntMap<Service> ();

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") {
				numBranches.Default = e.NewValue;
			} else if (e.Name == "done") {
				countBranches.Default = e.NewValue;
			} else if (e.Name.StartsWith ("item_")) {
				numBranches.Set (e.Name, e.NewValue, "item_");
			} else if (e.Name.StartsWith ("counted_")) {
				countBranches.Set (e.Name, e.NewValue, "counted_");
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			string sourceData;

			if (parameters.TryGetFallbackString (this.From, out sourceData)) {
				string[] splitData = Splitter.Split (sourceData);

				IInteraction lastParams = parameters;
				Service handler;
				int currentItem = 0;

				foreach (string split in splitData) {
					lastParams = new SplitInteraction (currentItem, parameters, To, split);
					handler = numBranches [currentItem++];
					if (handler != null) success &= handler.TryProcess (lastParams);
				}

				handler = countBranches [currentItem];
				if (handler != null) success &= handler.TryProcess (new SimpleInteraction(lastParams, "count", currentItem));
			}

			return success;
		}
	}
}

