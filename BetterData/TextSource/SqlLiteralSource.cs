using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;

namespace BetterData
{
	class SqlLiteralSource : SqlSource
	{
		private string Text;

		public override string GetText() {
			return this.Text;
		}

		public SqlLiteralSource (string textString)
		{
			this.Text = textString;
		}
	}


}

