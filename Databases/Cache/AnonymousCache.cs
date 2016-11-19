using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	public class AnonymousCache : SingleBranchService
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
			this.Data = null;
		}

		protected virtual byte[] Data { get; set; }

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			IOutgoingBodiedInteraction upstreamTarget;
			upstreamTarget = (IOutgoingBodiedInteraction)parameters.GetClosest (
				typeof(IOutgoingBodiedInteraction));

			if (timeUntilCacheDrop != TimeSpan.MaxValue) {
				if (DateTime.Now - lastUpdate > timeUntilCacheDrop) {
					Data = null;
				}
			}

			if (Data == null) {
				MemoryStream targetStream = new MemoryStream();
				SimpleOutgoingInteraction downstreamTarget = new SimpleOutgoingInteraction (
					targetStream, upstreamTarget.Encoding, parameters);

				success = WithBranch.TryProcess (downstreamTarget);		
				downstreamTarget.Done ();

				targetStream.Position = 0;

				if (downstreamTarget.HasWriter()) {
					using (StreamReader reader = new StreamReader(targetStream)) 
						Data = downstreamTarget.Encoding.GetBytes (reader.ReadToEnd ());
				} else {
					Data = new byte[targetStream.Length];
					targetStream.Read (Data, 0, Data.Length);
					targetStream.Dispose ();
				}

				lastUpdate = DateTime.Now;
			} 

			upstreamTarget.OutgoingBody.Write (Data, 0, Data.Length);

			return success;
		}
	}
}

