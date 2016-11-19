using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace BetterData
{
	class AutoSqlFileSource : GenerativeSqlFileSource
	{
		static class Clauses
		{
			public const string 
			SelectAll = "SELECT * FROM {0}",
			Where = "WHERE {0} = @{0}",

			Update = "UPDATE {0}",
			Set = "SET {0} = @{0}",
			ExtraSet = ", {0} = @{0}",

			Insert = "INSERT INTO {0}",
			Values = "VALUES",

			Delete = "DELETE FROM {0} ",

			SelectLastInsertId = ";\nSELECT LAST_INSERT_ID() AS last_insert_id;";
		}
		
		void ParseConditionClause (string[] sections, StreamWriter writer, int start)
		{
			if (sections.Length > start + 1) {
				if (sections [start].ToLower() == "by") {
					writer.WriteLine (Clauses.Where, sections [start + 1]);
				}
			}
		}

		protected override string Extension {
			get {
				return ".auto.sql";
			}
		}

		private bool WriteLastInsertId;

		public AutoSqlFileSource (string filePath, bool writeLastInsertId) : base(filePath)
		{
			this.WriteLastInsertId = writeLastInsertId;
		}

		void ParseSeries(string[] sections, StreamWriter writer, string format, int start) {
			writer.Write ('(');
			for (int i = 2; i < sections.Length; i++) {
				if (i > 2)
					writer.Write (',');
				writer.Write (format, sections [i]);
			}
			writer.WriteLine (')');
		}

		void DemandTablename (string[] sections)
		{
			if (sections.Length < 2) {
				throw new GeneratedSqlException ("Expected table name");
			} 
		}

		protected override void GenerateSqlForSections (string[] sections, StreamWriter writer)
		{
			string foundAction = sections [0].ToLower ();

			if (foundAction == "get") {
				// get thing by id
				DemandTablename (sections);
				writer.WriteLine (Clauses.SelectAll, sections [1]);

				ParseConditionClause (sections, writer, 2);
			} else if (foundAction == "set") {
				// 0   1     2    3  4
				// set thing name by id
				DemandTablename (sections);
				writer.WriteLine (Clauses.Update, sections [1]);

				if (sections.Length < 3) {
					throw new GeneratedSqlException ("Expected column name");
				} else {
					int position = 2;
					writer.WriteLine (Clauses.Set, sections [position++]);
					while(position < sections.Length - 2)
						writer.WriteLine (Clauses.ExtraSet, sections [position++]);
					ParseConditionClause (sections, writer, position);
				}
			} else if (foundAction == "add") {
				// add thing name color smell
				// 0   1     2    3     4
				DemandTablename (sections);
				writer.Write (Clauses.Insert, sections [1]);
				ParseSeries (sections, writer, "{0}", 2);
				writer.Write (Clauses.Values);
				ParseSeries (sections, writer, "@{0}", 2);
				if (WriteLastInsertId) {
					writer.Write (Clauses.SelectLastInsertId);
				}
			} else if (foundAction == "del") {
				// del thing by id
				DemandTablename (sections);
				writer.Write (Clauses.Delete, sections [1]);
				ParseConditionClause (sections, writer, 2);
			}
		}
	}
}
