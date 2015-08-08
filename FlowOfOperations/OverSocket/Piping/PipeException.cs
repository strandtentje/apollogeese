using System;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping
{
	class PipeException : Exception
	{
		public PipeException(string message) : base(message) {
		}
	}
}

