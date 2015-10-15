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
	class SqlFileSource : SqlSource
	{
		protected string FilePath;
		DateTime LastDate = DateTime.MaxValue; // this will absolutely break something
		string CachedText;

		public SqlFileSource (string filePath)
		{
			this.FilePath = filePath;
		}

		public override string GetText() {
			DateTime CurrentDate = File.GetLastWriteTime (this.FilePath);
			if (!CurrentDate.Equals (LastDate)) {
				this.UnchangedFlag = false;
				this.LastDate = CurrentDate;
				this.CachedText = File.ReadAllText (this.FilePath);
			}
			return this.CachedText;
		}
	}


}

