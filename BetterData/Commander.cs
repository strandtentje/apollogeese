using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace BetterData
{
	public abstract class Commander : NBranch
	{
		public string QueryName { 
			get { 
				return "";
			}
		}

		public string DatasourceName { 
			get {
				return ""; 
			} 
		}

		private IDbConnection connection {
			get; set; 
		}

		public IDbCommand GetCommand() {

		}

		protected override void Initialize (Settings settings)
		{
			connection = Connector.Find (settings.GetString("connection", "default"));
		}
	}

}

