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
using BorrehSoft.Utensils.Log;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils.Collections;

namespace BetterData
{
	public abstract class Communicator : Service
	{
		private string DSN;

		IDbConnection connection;
		DateTime timestamp;

		public bool InvalidConnection {
			get {
				DateTime nowish = DateTime.Now;
				TimeSpan dif = (nowish - timestamp);

				bool 
					noconnection = connection == null,
					notopen = noconnection || (connection.State != ConnectionState.Open),
					tooold = dif.TotalMinutes > 1;

				timestamp = nowish;

				return notopen || tooold;
			}
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public IDbConnection Connection { 
			get {
				if (InvalidConnection) {
					Secretary.Report (5, "Reviving connection because it was: too old or invalid");

					if (connection != null) {
						try {
							connection.Dispose();
						} catch(Exception ex) {
							Secretary.Report (5, "Failed to dispose of old one due to:", ex.Message);
						}
					}

					connection = Connector.Find (this.DatasourceName);
					connection.Open ();
				}

				return connection;
			}
		}

		public string DatasourceName { 
			get {
				return this.DSN; 
			}
			set {
				this.connection = null;
				this.DSN = value;
			}
		}

		protected override void Initialize (Settings settings)
		{			
			this.DatasourceName = settings.GetString ("connection", "default");
		}
				
		IDbCommand CreateCommand ()
		{
			IDbCommand newcommand = Connection.CreateCommand ();
			return newcommand;
		}

		void DestroyCommand (IDbCommand obj)
		{
			try {
				obj.Dispose();
			} catch(Exception ex) {
				Secretary.Report (5, "Couldn't dispose of command due to:", ex.Message);
			}
		}

		bool CanCommand (IDbCommand arg)
		{
			if (arg.Connection != this.Connection) {
				DestroyCommand (arg);
				return false;
			}
			return true;
		}

		BlockingPool<IDbCommand> commandPool;

		public virtual BlockingPool<IDbCommand> CommandPool {
			get {
				if (commandPool == null) {
					commandPool = new BlockingPool<IDbCommand> (
						1, CreateCommand, DestroyCommand, CanCommand);
				}

				return commandPool;
			}
		}
	}
}
