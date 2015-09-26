using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using BorrehSoft.Utensils.Log;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class QueryStats
	{
		public QueryStats (int rowCount);
		{
			this.RowCount = rowCount;
		}

		public int RowCount { get; private set; }
	}

}
