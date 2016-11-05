using System;
using BetterData;
using System.IO;
using System.Collections.Generic;

namespace BetterData
{
	class ModelSqlFileSource : GenerativeSqlFileSource
	{
		static class Clauses 
		{
			public const string 
			CreateTable = "CREATE TABLE IF NOT EXISTS {0} (",
			EndOfCreateTable = "\n)",

			PrimaryKey = "\t{0} INTEGER PRIMARY KEY AUTOINCREMENT",
			RegularColumn = ",\n\t{0} {1}",
			ForeignKey = " REFERENCES {0}({1})";
		}

		protected override string Extension {
			get {
				return ".model.sql";
			}
		}

		public ModelSqlFileSource(string filePath) : base (filePath)
		{
		}

		protected override void GenerateSqlForSections (string[] rawSections, StreamWriter writer)
		{
			var sections = new Queue<string> (rawSections);

			switch (sections.Dequeue().ToLower()) {
			case "table":
				GenerateTableCreate (sections, writer);
				break;
			default:
				throw new GeneratedSqlException ("Expected Table definition");
				break;
			}
		}

		void GenerateTableCreate (Queue<string> sections, StreamWriter writer)
		{
			var tableFootprint = sections.Dequeue ().Split ('.');
			var tableName = tableFootprint [0];
			var tablePk = tableFootprint [1];

			writer.WriteLine (Clauses.CreateTable, tableName);
			writer.Write (Clauses.PrimaryKey, tablePk);

			if (sections.Dequeue().ToLower() == "with") {
				while (sections.Count > 0) {
					GenerateColumnDefinition (sections.Dequeue (), writer);
				}				
			}

			writer.WriteLine (Clauses.EndOfCreateTable);
		}

		void GenerateColumnDefinition (string columnSpec, StreamWriter writer)
		{
			var columnFootprint = columnSpec.Split ('_');
			var referenceFootprint = columnFootprint [0].Split ('.');

			var columnType = "TEXT";

			var columnName = referenceFootprint [0];
			var referredTableName = "";
			if (referenceFootprint.Length > 1) {
				referredTableName = referenceFootprint [0];
				columnName = referenceFootprint [1];
				columnType = "INTEGER";
			}

			if (columnFootprint.Length > 1) {
				columnType = columnFootprint [1];
			}

			writer.Write (Clauses.RegularColumn, columnName, columnType);

			if (referredTableName.Length > 0) {
				writer.Write (Clauses.ForeignKey, referredTableName, columnName);
			}
		}
	}
}

