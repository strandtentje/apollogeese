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
	class TransactionInteraction : SimpleInteraction
	{
		public TransactionInteraction(IInteraction parent, BlockingPool<IDbCommand> commandPool) : base(parent) {
			this.CommandPool = commandPool;
		}

		public BlockingPool<IDbCommand> CommandPool {
			get;
			private set;
		}
	}
}
