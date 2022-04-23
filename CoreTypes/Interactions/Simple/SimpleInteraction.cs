using System;
using System.Linq;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Log;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class SimpleInteraction : Map<object>, IInteraction
	{
		private IInteraction parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.QuickInteraction"/> class.
		/// No parent or data to start with, useful for initiating.
		/// </summary>
		public SimpleInteraction ()
		{
			this.parent = null;
			this.ExceptionHandler = LoggingExceptionHandler.Handle;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.QuickInteraction"/> class.
		/// Will take a parent, extra data can be attached later, optionally.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public SimpleInteraction (IInteraction parent)
		{
			this.parent = parent;
			this.ExceptionHandler = parent.ExceptionHandler;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.QuickInteraction"/> class.
		/// Will take parent and extra data to base itself on.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="values">Values.</param>
		public SimpleInteraction (IInteraction parent, Map<object> values) : base(values)
		{
			this.parent = parent;
            if (parent?.ExceptionHandler != null)
                this.ExceptionHandler = parent.ExceptionHandler;
            else
                this.ExceptionHandler = DefaultEHandler;
		}

        public static void DefaultEHandler(
                Service s,
                IInteraction c,
                Exception e
            )
        {
            Secretary.Report(5, string.Format(
                "Exception occurred on an interaction " +
                "that had no exceptionhandler passed on. " +
                "{0} {1} {2}",
                s.GetType().Name, c.GetType().Name, e.Message));
        }

        public SimpleInteraction (IInteraction parent, string name, object value)
		{
			if (parent != null) {
				this.parent = parent;
				this.ExceptionHandler = parent.ExceptionHandler;
			}
			this [name] = value;
		}

		/// <summary>
		/// Gets the root interaction, typically the one that invoked the chain.
		/// This usually, in case of a website, ends you up with an IHttpInteraction.
		/// </summary>
		/// <value>
		/// The root interaction.
		/// </value>
		public IInteraction Root { 
			get { 
				if (parent == null)
					return this;
				else 
					return parent.Root; 
			} 
		}

		public ExceptionHandler ExceptionHandler { get; private set; }

		/// <summary>
		/// Gets the parent interaction.
		/// </summary>
		/// <value>
		/// The parent interaction.
		/// </value>
		public IInteraction Parent { get { return parent; } }

        public bool TryGetClosest(Type t, IInteraction limit, out IInteraction closest) 
        {
            for (closest = this; (closest != limit); closest = closest.Parent)
                if (t.IsAssignableFrom(closest.GetType()))
                    return true;

            return false;
        }

		public bool TryGetClosest(Type t, out IInteraction closest)
		{
            return TryGetClosest(t, null, out closest);
		}

		/// <summary>
		/// Gets the closest interation of type
		/// </summary>
		/// <returns>
		/// The closest.
		/// </returns>
		/// <param name='t'>
		/// T.
		/// </param>
		public IInteraction GetClosest (Type t)
		{
			IInteraction closest;

			if (t == null)
				throw new ArgumentNullException ("Type required for GetClosest");

			if (!TryGetClosest(t, out closest))
				throw new Exception(string.Format("No interaction in chain was of type", t.Name));

			return closest;
		}

		public IInteraction Clone (IInteraction parent)
		{
			return new SimpleInteraction(parent, this);
		}

		public bool TryGetFallback (string id, out object luggage)
		{
			if (this.TryGetValue(id, out luggage))
				return true;

			if (this.parent != null)
				return this.parent.TryGetFallback(id, out luggage);

			luggage = null;
			return false;
		}

		public bool TryGetFallbackString(string id, out string luggage)
		{
			if (this.TryGetString(id, out luggage))
				return true;

			if (this.parent != null)
				return this.parent.TryGetFallbackString(id, out luggage);

			luggage = "";
			return false;
		}
	}
}

