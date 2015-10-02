using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using MySql.Data.MySqlClient;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;

namespace BetterData
{
	class MissingConnectorException : Exception
	{
		public MissingConnectorException (string type) : base(
			string.Format("Connector for database type '{0}' was not found.", type))
		{

		}
	}



}

