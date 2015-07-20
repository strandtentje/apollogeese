using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	public class Cache : Service
	{
		public override string Description {
			get {
				return "Caches IOutgoingInteractions";
			}
		}

		TimeSpan cacheLifetime;
		DateTime lastUpdate = DateTime.Now;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["lifetime"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			string lifetimeString = "";
			bool hasLifetime = false;

			if (modSettings.Has ("lifetime")) {
				lifetimeString = modSettings.GetString ("lifetime");
				hasLifetime = true;
			}

			if (hasLifetime) {
				cacheLifetime = TimeSpan.Parse (lifetimeString);
			} else {
				// that will take a while surely.
				cacheLifetime = TimeSpan.MaxValue;
			}
		}

		private Service begin;
		private bool isStringCache;
		private byte[] binaryData = null;
		private string stringData = null;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin")
				begin = e.NewValue;
		}
	
		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			IOutgoingBodiedInteraction upstreamTarget;
			upstreamTarget = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));

			if (DateTime.Now - lastUpdate > cacheLifetime) {
				binaryData = null;
				stringData = null;
			}

			if ((binaryData == null) && (stringData == null)) {
				MemoryStream targetStream = new MemoryStream();
				QuickOutgoingInteraction downstreamTarget = new QuickOutgoingInteraction (targetStream, parameters);

				success = begin.TryProcess (downstreamTarget);		
				downstreamTarget.Done ();

				targetStream.Position = 0;

				if (downstreamTarget.HasWriter()) {
					isStringCache = true;
					using (StreamReader reader = new StreamReader(targetStream)) 
						stringData = reader.ReadToEnd ();
				} else {
					binaryData = new byte[targetStream.Length];
					targetStream.Read (binaryData, 0, binaryData.Length);
					targetStream.Dispose ();
				}

				lastUpdate = DateTime.Now;
			} 

			if (isStringCache) {
				upstreamTarget.GetOutgoingBodyWriter ().Write (stringData);
				upstreamTarget.GetOutgoingBodyWriter ().Flush ();
			} else {
				upstreamTarget.OutgoingBody.Write (binaryData, 0, binaryData.Length);
			}

			return success;
		}
	}
}

