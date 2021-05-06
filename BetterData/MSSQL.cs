using System;
using System.Data;
using System.Data.SqlClient;
using BetterData;

namespace PersistentData
{
    public class MSSQL : Connector
    {
        public override string ConnectionStringTemplate
        {
            get
            {
                return "Data Source=localhost;Initial Catalog={0};User ID={0};Password={0}";
            }
        }

        public override IDbConnection GetNewConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }
    }
}
