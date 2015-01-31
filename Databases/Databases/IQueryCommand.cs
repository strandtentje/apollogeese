using System;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Data
{
	/// <summary>
	/// Query Command interface.
	/// </summary>
	public interface IQueryCommand
	{		
		/// <summary>
		/// Underlying IDbCommand for this query command.
		/// </summary>
		/// <value>The command.</value>
		IDbCommand Command { get; }

		/// <summary>
		/// Sets a command parameter.
		/// </summary>
		/// <returns>
		/// The command parameter.
		/// </returns>
		/// <param name='command'>
		/// Command.
		/// </param>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		IDbDataParameter SetParameter (string name, object value);

		/// <summary>
		/// Run this query
		/// </summary>
		IDataReader Run();	
	}
}

