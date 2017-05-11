using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ServiceDoc : Service
	{
		public override string Description {
			get {
				return "Service Documentation Retrieval";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			
		}
	}
}

