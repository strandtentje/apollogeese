using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace RegularHttpServer
{
	public class CacheService : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] { "http" };
			}
		}

		public override string Description {
			get {
				return "Caches requests";
			}
		}

		public override void Initialize (Settings modSettings)
		{

		}

		Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();

		public override bool Process (HttpListenerContext context, Parameters parameters)
		{
			byte[] cachedData;
			string url = context.Request.RawUrl;
			Stream outStream = context.Response.OutputStream;

			if (cache.ContainsKey (url)) {
				cachedData = cache [url];
				outStream.Write (cachedData, 0, cachedData.Length);
			} else {
				long offset = outStream.Position;
				RunBranch ("http", context, parameters);

				long length = outStream.Position - offset;
				outStream.Position = offset;
				cachedData = new byte[length];
				outStream.Read (cachedData, 0, (int)length);

				cache.Add (url, cachedData);
			}

			return true;
		}
	}
}

