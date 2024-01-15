using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;

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

