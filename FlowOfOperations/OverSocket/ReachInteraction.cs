using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using System.Reflection;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	class ReachInteraction : IInteraction
	{
		private Pipe<Command> pipe;
	
		public ReachInteraction (Pipe<Command> pipe)
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

		bool GetTypenameTryString(string id, out string luggage, out string typename) {
			pipe.SendCommand (Command.Type);
			pipe.SendString (id);
			typename = pipe.ReceiveString ();

			if (typename == typeof(string).Name) {
				pipe.SendCommand (Command.String);
				pipe.SendString (id);
				luggage = pipe.ReceiveString ();
				return true;
			} else {
				luggage = "";
				return false;
			}
		}

		object GetValue(string id, string typename) {
			Type targetType = Type.GetType (typename);

			MethodInfo parseMethod = targetType.GetMethod ("Parse", BindingFlags.Static);

			pipe.SendCommand (Command.Compose);
			pipe.SendString (id);

			return parseMethod.Invoke (null, pipe.ReceiveString ());
		}

		bool TryGetString(string id, out string luggage) {
			string butt;
			return GetTypenameTryString (id, out luggage, out butt);
		}

		bool TryGetValue(string id, out object luggage) {
			string typename;

			if (!GetTypenameTryString (id, out luggage, out typename)) {


				luggage = GetValue (id, typename);
			}
		}

		/// <summary>
		/// Gets or sets the value with the specified name.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		object this [string name] { 
			get {
				object result;
				if (TryGetValue (name, out result)) {
					return result;
				} else {
					return null;
				}
			}
		}

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
