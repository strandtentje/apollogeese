using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections;

namespace ExternalData
{
	public abstract class NameValueService : ExternalDataService
	{
		public Service None {
			get;
			set;
		}

		public Service Iterator {
			get;
			set;
		}

		public Service Mapped {
			get;
			set;
		}

		public bool DoMapping {
			get;
			set;
		}

		public bool DoIterate {
			get;
			set;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "none")
				this.None = e.NewValue;
			else if (e.Name == "iterator") {
				this.Iterator = e.NewValue;
				this.DoIterate = this.Iterator != null;
			}
			else if (e.Name == "mapped") {
				this.Mapped = e.NewValue;
				this.DoMapping = this.Mapped != null;
			}
		}

		protected bool TryReportPair(Map<object> summary, NameValueInteraction input) {
			bool success = true;

			if (this.DoIterate) 
				success &= this.Iterator.TryProcess (input);
			if (this.DoMapping)
				summary [input.Name] = input.Value;
			if (this.Branches.Has (input.Name))
				success &= this.Branches [input.Name].TryProcess (input);

			return success;
		}
	}
}

