using System;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    [Serializable]
    internal class NoCaptureScopeException : Exception
    {
        public NoCaptureScopeException(string message) : base(message)
        {
        }
    }
}
