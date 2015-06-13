using System;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	class PipeException : Exception
	{
		public PipeException(string message) : base(message) {
		}
	}
}

