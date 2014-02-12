using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings.FileListing
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

		protected override void Initialize (Settings modSettings)
		{
			if (modSettings ["mappings"] == null)
				throw new MissingSettingException ("where type is \"fileserver\"", "mappings", "a mapping from a URL-path to filesystem path");

			this.mappings.Clear ();

			foreach (object s in (List<object>)modSettings ["mappings"]) {
				this.mappings.Add (new Mapping((Settings)s));
			}
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters;

			foreach (Mapping mapping in mappings) {
				if (mapping.Follow (parameters))
					return true;
			}

			return false;
		}
	}
}