using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using System.Reflection;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	class ReachInteraction : IInteraction
	{
		public const string NullTypeName = "[nothing]";

		private Pipe pipe;
	
		public ReachInteraction (Pipe pipe)
		{
			this.pipe = pipe;
		}

		public ExceptionHandler ExceptionHandler {
			get {
				return LoggingExceptionHandler.Handle;
			}
		}

		public IInteraction Root { get { return this; } }

		public IInteraction Parent { get { return null; } }

		public IInteraction GetClosest(Type t) {
			IInteraction closest = null;

			if (t.Equals (this.GetType ())) {
				closest = this;
			} 

			return closest;
		}


        public bool TryGetClosest(Type t, IInteraction limit, out IInteraction closest)
        {
            bool success = t.Equals(this.GetType());
            closest = (success ? this : null);

            return success;
        }

		public bool TryGetClosest (Type t, out IInteraction closest) {
            return TryGetClosest(t, null, out closest);
		}

		public IInteraction Clone(IInteraction parent) {
			throw new UnclonableException ();
		}

		public bool GetTypenameTryString(string id, out string luggage, out string typename) {
			bool success = false;
			luggage = "";

			pipe.SendCommand (Command.Type);
			pipe.SendString (id);
			typename = pipe.ReceiveString ();

			if (typename == typeof(string).Name) {
				pipe.SendCommand (Command.String);
				pipe.SendString (id);
				luggage = pipe.ReceiveString ();
				success = true;
			}

			return success;
		}

		public object GetValue(string id, string typename) {
			Type targetType = Type.GetType (typename);

			BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			MethodInfo parseMethod = targetType.GetMethod (
				"Parse", new Type[] { typeof(string) });

			pipe.SendCommand (Command.Compose);
			pipe.SendString (id);

			return parseMethod.Invoke (null, new object[] { pipe.ReceiveString () });
		}

		public bool TryGetString(string id, out string luggage) {
			string butt;
			return GetTypenameTryString (id, out luggage, out butt);
		}

		public bool TryGetValue(string id, out object luggage) {
			bool success = true;
			string typename;
			string luggageText;

			if (GetTypenameTryString (id, out luggageText, out typename)) {
				luggage = luggageText;
			} else {
				if (typename == NullTypeName) {
					luggage = null;
					success = false;
				} else {
					luggage = GetValue (id, typename);
				}
			}

			return success;
		}

		/// <summary>
		/// Gets or sets the value with the specified name.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public object this [string name] { 
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
		public bool TryGetFallbackString(string id, out string luggage) {
			return TryGetString (id, out luggage);
		}

		/// <summary>
		/// Scans from here to ancestors for data at specified name. Returns false if nothing found.
		/// </summary>
		/// <returns><c>true</c>, if get fallback was tryed, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="luggage">Luggage.</param>
		public bool TryGetFallback (string id, out object luggage) {
			return TryGetValue (id, out luggage);
		}
		 
	}
}
