using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public static class Closest<T> where T : IInteraction
	{
		public class InteractionNotFoundException : Exception 
		{
			public InteractionNotFoundException(Type ofType) : base(
				string.Format(
					"Interaction of type {0} not found", 
					ofType.Name
				)
			) { }
		}

		public static T From(IInteraction parameters)
		{
			IInteraction candidate;
			if (parameters.TryGetClosest (typeof(T), out candidate)) {
				return (T)candidate;
			} else {
				throw new InteractionNotFoundException (typeof(T));
			}
		}
	}
}

