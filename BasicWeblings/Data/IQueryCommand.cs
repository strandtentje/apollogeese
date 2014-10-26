using System;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Data
{
	public interface IQueryCommand
	{		
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

		IDataReader Run();	
	}
}

