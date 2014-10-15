using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class LookupWriter : Service
	{
		string LookupName {
			get;
			set;
		}

		public override string Description {
			get {
				return "Lookup " + this.LookupName;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}
	}
}

