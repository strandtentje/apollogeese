using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class BareInteraction : IInteraction
	{
		protected IInteraction parent;

		public BareInteraction ()
		{
			this.parent = null;
			this.ExceptionHandler = LoggingExceptionHandler.Handle;
		}

		public BareInteraction (IInteraction parent)
		{
			this.parent = parent;
			this.ExceptionHandler = parent.ExceptionHandler;
		}

		public IInteraction Root { 
			get { 
				if (parent == null)
					return this;
				else
					return parent.Root; 
			} 
		}

		public ExceptionHandler ExceptionHandler { get; protected set; }

		public IInteraction Parent { get { return parent; } }

		public bool TryGetClosest (Type t, IInteraction limit, out IInteraction closest)
		{
			for (closest = this; (closest != limit); closest = closest.Parent)
				if (t.IsAssignableFrom (closest.GetType ()))
					return true;

			return false;
		}

		public bool TryGetClosest (Type t, out IInteraction closest)
		{
			return TryGetClosest (t, null, out closest);
		}

		public IInteraction GetClosest (Type t)
		{
			IInteraction closest;

			if (t == null)
				throw new ArgumentNullException ("Type required for GetClosest");

			if (!TryGetClosest (t, out closest))
				throw new Exception (string.Format ("No interaction in chain was of type", t.Name));

			return closest;
		}

		public bool TryGetFallback (string id, out object luggage)
		{
			if (this.parent != null)
				return this.parent.TryGetFallback (id, out luggage);

			luggage = null;
			return false;
		}

		public bool TryGetFallbackString (string id, out string luggage)
		{
			if (this.parent != null)
				return this.parent.TryGetFallbackString (id, out luggage);

			luggage = "";
			return false;
		}

		public bool TryGetString (string id, out string luggage)
		{ 
			luggage = null;
			return false;
		}

		public bool TryGetValue (string id, out object luggage)
		{
			luggage = null;
			return false;
		}

		public object this [string name] { get { return null; } }

		public abstract IInteraction Clone (IInteraction parent);
	}
}

