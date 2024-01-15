using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utilities.Log;
using System.Text.RegularExpressions;
using BorrehSoft.Utilities.Collections;

namespace BetterData
{
	class TransactionInteraction : SimpleInteraction
	{
        public TransactionInteraction(IDbConnection connection, string datasourceName, IInteraction parent) : base(parent)
        {
            this.Connection = connection;
            this.DatasourceName = datasourceName;
        }

        public IDbConnection Connection { get; set; }

        public string DatasourceName { get; set; }
    }
}
