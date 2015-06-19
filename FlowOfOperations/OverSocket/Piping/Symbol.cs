using System;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping
{
	/// <summary>
	/// Pipe symbol
	/// </summary>
	public enum Symbol : byte
	{
		/// <summary>
		/// Handshake, needs to happen both ways
		/// </summary>
		Hi = 1,
		/// <summary>
		/// Upcoming data will be int
		/// </summary>
		Int = 2,
		/// <summary>
		/// Upcoming data will be length and string
		/// </summary>
		String = 3,
		/// <summary>
		/// Upcoming data will be command short
		/// </summary>
		Command = 4
	}
}

