using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	public class HttpMethod : Service
	{
		public override string Description {
			get {
				return "HttpMethod brancher";
			}
		}

		static Dictionary<string, bool> cachedWhitelist = null;

		private Dictionary<string, bool> Whitelist {
			get {
				if (cachedWhitelist == null) {
					cachedWhitelist = new Dictionary<string, bool> ();
					cachedWhitelist.Add ("GET", true);
					cachedWhitelist.Add ("HEAD", true);
					cachedWhitelist.Add ("POST", true);
					cachedWhitelist.Add ("PUT", true);
					cachedWhitelist.Add ("DELETE", true);
					cachedWhitelist.Add ("TRACE", true);
					cachedWhitelist.Add ("OPTIONS", true);
					cachedWhitelist.Add ("CONNECT", true);
					cachedWhitelist.Add ("PATCH", true);
				}

				return cachedWhitelist;
			}
		}

		private bool IsMethodValid(string method) {
			bool success = false;

			if (method.Length > 2) {
				if (method.Length < 8) {
					success = Whitelist [method.ToUpper()];
				}
			}

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;

			HttpInteraction interaction = (HttpInteraction)parameters.GetClosest (typeof(HttpInteraction));

			string method = interaction.RequestMethod;

			if (IsMethodValid (method)) {
				success &= Branches [method.ToLower()].TryProcess (parameters);
			} else {
				interaction.SetStatusCode (405);
			}

			return success;
		}
	}
}

