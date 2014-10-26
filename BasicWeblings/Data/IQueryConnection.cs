using System;
using System.Data;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings.Data
{
	public interface IQueryConnection
	{
		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>
		/// The connection string.
		/// </value>
		string ConnectionString { get; }
		
		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		IDbConnection Connection { get; }

		/// <summary>
		/// Gets the query text.
		/// </summary>
		/// <value>
		/// The query text.
		/// </value>
		string DefaultQueryText { get; }

		/// <summary>
		/// Gets the ordered parameters.
		/// </summary>
		/// <value>
		/// The ordered parameters.
		/// </value>
		List<string> DefaultOrderedParameters { get; }

		void SetDefaultCommandQuery(string QueryTextFile, List<object> queryParameters = null);
		
		IQueryCommand GetDefaultCommand();
	}
}

