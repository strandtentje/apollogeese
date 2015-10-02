using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace BetterData
{
	class CommandPool
	{
		public CommandPool (string datasourceName)
		{
			this.DatasourceName = datasourceName;
		}

		public string DatasourceName { get; set; }
	}


}

