using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BasicHttpServer
{
	public class HttpContent : Service
	{
		public override string Description {
			get {
				return "Response preparation for content";
			}
		}

		protected override void Initialize (Settings settings)
		{
			
		}

		private Service Content = Stub;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			switch (e.Name) {
			case "http":
			case "data":
			case "content":
				this.Content = e.NewValue ?? Stub;
				break;
			default:				
				break;
			}
		}
	
		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction httpInteraction = Closest<IHttpInteraction>.From (parameters);
			string contentType = Fallback<string>.From (parameters, "contenttype");
			long length = Fallback<long>.From (parameters, "contentlength");

			httpInteraction.SetContentType (contentType);
			httpInteraction.SetContentLength (length);
			httpInteraction.PurgeBuffer ();

			return this.Content.TryProcess (parameters);
		}
	}
}

