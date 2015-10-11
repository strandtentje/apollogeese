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
	class GeneratedSqlTextFile : TextFileSource
	{
		class GeneratedSqlException : Exception
		{
			public GeneratedSqlException (string message) : base(message)
			{

			}
		}

		static class Clauses
		{
			public const string SelectAll = "SELECT * FROM {0}";
			public const string Where = "WHERE {0} = @{0}";

			public const string Update = "UPDATE {0}";
			public const string Set = "SET {0} = @{0}";

			public const string Insert = "INSERT INTO {0}";
			public const string Values = "VALUES";

			public const string Delete = "DELETE FROM {0}";
		}
		
		void ParseConditionClause (string[] sections, StreamWriter writer, int start)
		{
			if (sections.Length > start + 1) {
				if (sections [start].ToLower() == "by") {
					writer.WriteLine (Clauses.Where, sections [start + 1]);
				}
			}
		}

		public GeneratedSqlTextFile (string filePath) : base(filePath)
		{
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

		void ParseActionClause (string[] sections, StreamWriter writer)
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
					writer.WriteLine (Clauses.Set, sections [2]);
					ParseConditionClause (sections, writer, 3);
				}
			} else if (foundAction == "add") {
				// add thing name color smell
				// 0   1     2    3     4
				DemandTablename (sections);
				writer.Write (Clauses.Insert, sections [1]);
				ParseSeries (sections, writer, "{0}", 2);
				writer.Write (Clauses.Values);
				ParseSeries (sections, writer, "@{0}", 2);
			} else if (foundAction == "del") {
				// del thing by id
				DemandTablename (sections);
				writer.Write (Clauses.Delete, sections [1]);
				ParseConditionClause (sections, writer, 2);
			}
		}

		public override string GetText ()
		{
			if (!File.Exists (FilePath)) {
				FileInfo info = new FileInfo (FilePath);

				using (StreamWriter writer = new StreamWriter(info.OpenWrite())) {
					string TinyInstruction = info.Name.Substring (
						0, info.Name.Length - ".auto.sql".Length);
					ParseActionClause (
						TinyInstruction.Split(' '), writer);
				}
			}
			return base.GetText ();
		}
	}
}
