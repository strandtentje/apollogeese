using System;
using BorrehSoft.ApolloGeese.Duckling;
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

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	class QueryException : Exception
	{
		public QueryException (string message) : base(message)
		{

		}
	}
}
