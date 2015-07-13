using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	public class HttpMethod : Service
	{
		static Dictionary<string, bool> whitelist = null;

		private Dictionary<string, bool> Whitelist {
			get {
				if (whitelist == null) {
					whitelist = new Dictionary<string, bool> ();
					whitelist.Add ("GET", true);
					whitelist.Add ("HEAD", true);
					whitelist.Add ("POST", true);
					whitelist.Add ("PUT", true);
					whitelist.Add ("DELETE", true);
					whitelist.Add ("TRACE", true);
					whitelist.Add ("OPTIONS", true);
					whitelist.Add ("CONNECT", true);
					whitelist.Add ("PATCH", true);
				}

				return whitelist;
			}
		}

		private bool IsMethodValid(string method) {
			bool success = false;

			if (method.Length > 2) {
				if (method.Length < 8) {
					success = whitelist [method.ToUpper()];
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
				interaction.SetStatuscode (405);
			}

			Branches [interaction.RequestMethod.ToLower ()].TryProcess (parameters);
		}
	}
}

