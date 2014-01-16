using System;
using MySql.Data;
using BorrehSoft.ApolloGeese.Duckling;
using System.Net;
using BorrehSoft.Utensils.Settings;

namespace Datatables
{
	public class Tableservice : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] {};
			}
		}

		public override string Description {
			get {
				return "Turns a query-result into an HTML table";
			}
		}
		
		protected override void Initialize (Settings modSettings)
		{


		}

		protected override bool Process (Interaction parameters)
		{

			return false;
		}


	}
}
	