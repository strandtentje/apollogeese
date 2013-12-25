using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;

namespace BorrehSoft.Extensions.FileServer
{
	/// <summary>
	/// File service.
	/// </summary>
	public class FileService : Service
	{
		public override string Description {
			get {
				return "Fileserver";
			}
		}

		static string[] branches = new string[] { };

		public override string[] AdvertisedBranches {
			get {
				return branches;
			}
		}

		List<Mapping> mappings = new List<Mapping>();

		public override void Initialize (Settings modSettings)
		{
			if (modSettings ["mappings"] == null)
				throw new MissingSettingException ("where type is \"fileserver\"", "mappings", "a mapping from a URL-path to filesystem path");

			foreach (object s in (List<object>)modSettings ["mappings"]) {
				this.mappings.Add (new Mapping((Settings)s));
			}
		}

		public override bool Process (HttpListenerContext context, Parameters parameters)
		{
			foreach (Mapping mapping in mappings) {
				if (mapping.Follow (context.Request.RawUrl, context.Response))
					return true;
			}

			return false;
		}
	}
}