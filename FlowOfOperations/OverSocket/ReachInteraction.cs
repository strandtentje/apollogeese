using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	class ReachInteraction : IInteraction
	{
		private Pipe pipe;
	
		public ReachInteraction (Pipe pipe)
		{
			this.pipe = pipe;
		}

		public IInteraction Root { get { return this; } }

		public IInteraction Parent { get { return null; } }

		public IInteraction GetClosest(Type t) {
			if (t.Equals (this.GetType ())) {
				return this;
			}
		}

		bool TryGetClosest (Type t, out IInteraction closest) {
			if (t.Equals (this.GetType ())) {
				closest = this;
				return true;
			}
			return false;
		}

		IInteraction Clone(IInteraction parent) {
			throw new UnclonableException ();
		}

		bool TryGetString(string id, out string luggage) {
			this.pipe.Has(
		}

		/// <summary>
		/// Tries to get an object from the luggage
		/// </summary>
		/// <returns>
		/// Ttrue if acquisition of object has succeeded
		/// </returns>
		/// <param name='id'>
		/// The name of the object in the luggage
		/// </param>
		/// <param name='luggage'>
		/// The variable into which the object will be loaded
		/// </param>
		bool TryGetValue(string id, out object luggage);

		/// <summary>
		/// Gets or sets the value with the specified name.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		object this [string name] { get; set; }

		/// <summary>
		/// Same as TryGetFallback, but only succeeds if string is found.
		/// </summary>
		/// <returns><c>true</c>, if get fallback string was tryed, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="luggage">Luggage.</param>
		bool TryGetFallbackString(string id, out string luggage);

		/// <summary>
		/// Scans from here to ancestors for data at specified name. Returns false if nothing found.
		/// </summary>
		/// <returns><c>true</c>, if get fallback was tryed, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="luggage">Luggage.</param>
		bool TryGetFallback (string id, out object luggage);
	}
}
