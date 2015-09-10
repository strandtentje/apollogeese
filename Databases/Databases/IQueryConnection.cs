using System;
using System.Data;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	/// <summary>
	/// Querying-Connection interface
	/// </summary>
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

		/// <summary>
		/// Sets the default query for this connection
		/// </summary>
		/// <param name="QueryText">Query text.</param>
		/// <param name="queryParameters">Query parameters.</param>
		void SetDefaultCommandQuery(string QueryText, List<string> queryParameters = null);

		/// <summary>
		/// Gets the default command for this connection
		/// </summary>
		/// <returns>The default command.</returns>
		IQueryCommand GetDefaultCommand();
	}
}

