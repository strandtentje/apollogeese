using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public enum Method { POST, GET }

	public interface IMethodInteraction : IInteraction
	{
		Method GetMethod();
	}
}

