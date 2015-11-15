using System;
using BorrehSoft.ApolloGeese.CoreTypes;
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

		[Instruction("Lifetime of Cache.")]
		public string CacheLifetime { 
			get {
				if (timeUntilCacheDrop == TimeSpan.MaxValue)
					return "";
				else
					return timeUntilCacheDrop.ToString ();
			}
			set {
				if (value.Length == 0)
					timeUntilCacheDrop = TimeSpan.MaxValue;
				else
					timeUntilCacheDrop = TimeSpan.Parse (value);
			}
		}

		TimeSpan timeUntilCacheDrop;
		DateTime lastUpdate = DateTime.Now;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["lifetime"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.CacheLifetime = modSettings.GetString ("lifetime", "");
		}

		private Service begin;
		private bool isStringCache;

		protected virtual byte[] binaryData { get; set; }
		protected virtual string stringData { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin")
				begin = e.NewValue;
		}
	
		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			IOutgoingBodiedInteraction upstreamTarget;
			upstreamTarget = (IOutgoingBodiedInteraction)parameters.GetClosest (
				typeof(IOutgoingBodiedInteraction));

			if (timeUntilCacheDrop != TimeSpan.MaxValue) {
				if (DateTime.Now - lastUpdate > timeUntilCacheDrop) {
					binaryData = null;
					stringData = null;
				}
			}

			if ((binaryData == null) && (stringData == null)) {
				MemoryStream targetStream = new MemoryStream();
				SimpleOutgoingInteraction downstreamTarget = new SimpleOutgoingInteraction (
					targetStream, upstreamTarget.Encoding, parameters);

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

			upstreamTarget.OutgoingBody.Write (binaryData, 0, binaryData.Length);

			return success;
		}
	}
}

