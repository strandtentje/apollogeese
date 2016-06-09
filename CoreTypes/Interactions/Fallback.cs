using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public static class Fallback<T>
	{
		public class VariableNotFoundException : Exception
		{
			public VariableNotFoundException(string name, Type ofType) : base(
				string.Format(
					"Variable with name {0} of type {1} not found",
					name, ofType.Name
				)
			) { }
		}

		public static T From(IInteraction parameters, string name)
		{
			object candidate;
			if (parameters.TryGetFallback (name, out candidate) && (candidate is T)) {
				return (T)candidate;
			} else {
				throw new VariableNotFoundException (name, typeof(T));
			}
		}
	}
}

