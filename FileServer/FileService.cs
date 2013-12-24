using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;
using Duple = BorrehSoft.Utensils.Tuple<string, string>;

namespace BorrehSoft.Extensions.FileServer
{
	public class FileService : Service
	{
		public override string Name {
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

		List<Duple> mappings = new List<Duple>();


		public override void Initialize (Settings modSettings)
		{
			if (modSettings ["mappings"] == null)
				throw new MissingSettingException ("where type is \"fileserver\"", "mappings", "an array of from=/to= pairs");

			Settings[] mappings = (Settings[])modSettings ["mappings"];

			foreach (Settings s in mappings) {
				this.mappings.Add (
					new Duple (
						(string)s["from"], 
						(string)s["to"]
					)
				);
			}
		}

		public override bool Process (HttpListenerRequest request, Parameters parameters)
		{

			return true;
		}
	}
}