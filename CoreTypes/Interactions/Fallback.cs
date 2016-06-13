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

		public static T From(IInteraction parameters, params string[] names)
		{
			object candidate;

			foreach (string name in names) {
				if (parameters.TryGetFallback (name, out candidate) && (candidate is T)) {
					return (T)candidate;
				}	
			}

			throw new VariableNotFoundException (
				string.Join(" or ", names), 
				typeof(T)
			);
		}
	}
}

