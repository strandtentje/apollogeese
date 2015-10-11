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
	class PlainTextSource : ITextSource
	{
		private string Text;

		public string GetText() {
			return this.Text;
		}

		public PlainTextSource (string textString)
		{
			this.Text = textString;
		}
	}


}

