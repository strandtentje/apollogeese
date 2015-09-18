using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class SQLList : Service
	{
		public List<string> Columns {
			get;
			set;
		}

		string TablenameSource {
			get;
			set;
		}

		string Where {
			get;
			set;
		}

		public override string Description {
			get {
				return "Generic table data getter";
			}
		}

		string Format {
			get;
			set;
		}

		string WhereFormat {
			get;
			set;
		}

		string tablenamePattern;

		Regex TablenameRegex {
			get;
			set;
		}

		string TablenamePattern {
			get {
				return this.tablenamePattern;
			} set {
				this.tablenamePattern = value;

				this.TablenameRegex = new Regex (value);
			}
		}

		string Tablename {
			get;
			set;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["tablename"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Columns = modSettings.GetStringList ("columns", new string[] { "*" });
			this.Tablename = modSettings ["tablename"];
			this.TablenameSource = modSettings.GetString ("tablenamesource", "tablename");
			this.TablenamePattern = modSettings.GetString("tablenamepattern", @"^\w+$");
			this.Where = modSettings.GetString ("where", "");
			this.Format = modSettings.GetString ("format", "SELECT {0} FROM {1} {2}");
			this.WhereFormat = modSettings.GetString ("whereformat", "WHERE {2}");
		}

		Service MissingTable = Stub;

		Service IllegalName = Stub;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "missing") {
				this.MissingTable = e.NewValue;
			} else if (e.Name == "illegal") {
				this.IllegalName = e.NewValue;
			}
		}

		Map<Service> queriers = new Map<Service>();

		Service CreateTableQuerier (string tableName)
		{
			Service tableQuerier = new SQL ();

			Settings newSettings = GetSettings ().Clone ();

			string whereClause = "";

			if (this.Where.Length > 0) {
				whereClause = string.Format (this.WhereFormat, this.Where);
			}

			newSettings ["querytext"] = string.Format (
				this.Format, string.Join (
					", ", this.Columns), 
				tableName, whereClause);

			tableQuerier.SetBranches (this.Branches);
			tableQuerier.SetSettings (newSettings);

			return tableQuerier;
		}

		protected Service GetTableQuerier(string tableName) {
			Service foundQuerier;

			if (queriers.Has (tableName)) {
				foundQuerier = queriers [tableName];
			} else if (this.TablenameRegex.IsMatch (tableName)) {
				foundQuerier = CreateTableQuerier (tableName);

				queriers [tableName] = foundQuerier;
			} else {
				return IllegalName;
			}

			return foundQuerier;
		}

		protected override bool Process (IInteraction parameters)
		{
			string tableName;

			Service foundService;

			if (this.Tablename != null) {
				foundService = this.GetTableQuerier (this.Tablename);
			} else if (parameters.TryGetFallbackString (this.TablenameSource, out tableName)) {
				foundService = this.GetTableQuerier (tableName);
			} else {
				foundService = this.MissingTable;
			}

			return foundService.TryProcess (parameters);
		}
	}
}

