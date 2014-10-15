using System;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public class QuickInteraction : Map<object>, IInteraction
	{
		private IInteraction parent;

		public QuickInteraction ()
		{
			this.parent = null;
		}

		public QuickInteraction (IInteraction parent)
		{
			this.parent = parent;
		}

		public QuickInteraction (IInteraction parent, Map<object> values) : base(values)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Gets the root interaction, typically the one that invoked the chain.
		/// This usually, in case of a website, ends you up with an IHttpInteraction.
		/// </summary>
		/// <value>
		/// The root interaction.
		/// </value>
		public IInteraction Root { get { return parent.Root; } }

		/// <summary>
		/// Gets the parent interaction.
		/// </summary>
		/// <value>
		/// The parent interaction.
		/// </value>
		public IInteraction Parent { get { return parent; } }

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
			for (IInteraction current = this; (current != null); current = current.Parent)
				if (t.IsAssignableFrom(current.GetType())) 
					return current;


			throw new Exception("No interaction in chain was of specified type");
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
