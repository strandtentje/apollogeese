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

		string ContentType;
		bool SendLength;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["contenttype"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			this.ContentType = settings.GetString ("contenttype", "");
			this.SendLength = settings.GetBool ("sendlength", true);
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
			string contentType;

			if (this.ContentType.Length > 0) {
				contentType = this.ContentType;
			} else {
				contentType = Fallback<string>.From (parameters, "contenttype");
			};

			httpInteraction.SetContentType (contentType);

			if (this.SendLength) {
				long length = Fallback<long>.From (parameters, "contentlength");
				httpInteraction.SetContentLength (length);
			}

			httpInteraction.PurgeBuffer ();

			return this.Content.TryProcess (parameters);
		}
	}
}

