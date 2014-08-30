using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.ComponentModel;
using System.Collections.Generic;

namespace GeeseUI.Graph
{
	/// <summary>
	/// ApolloGeese structure.
	/// </summary>
	public class Structure : List<Representation>
	{
		/// <summary>
		/// Gets the starting point of the structure
		/// </summary>
		/// <value>
		/// The starting point.
		/// </value>
		public Service StartingPoint { get; private set; }

		/// <summary>
		/// Sets the starting point.
		/// </summary>
		/// <param name='startingPoint'>
		/// Starting point.
		/// </param>
		public void SetStartingPoint (Service startingPoint)
		{
			this.Clear();
			LoadStructure(startingPoint);			
			this.StartingPoint = startingPoint;
		}

		/// <summary>
		/// Loads the structure.
		/// </summary>
		/// <returns>
		/// The structure.
		/// </returns>
		/// <param name='node'>
		/// Node.
		/// </param>
		private NodeRepresentation LoadStructure (Service node, int depth = 0)
		{
			NodeRepresentation nodeRepresentation = new NodeRepresentation (node, depth);
			this.Add (nodeRepresentation);

			foreach (Service s in node.Branches.Dictionary.Values) {
				this.Add(new ConnectionRepresentation(
					nodeRepresentation, LoadStructure(s, depth + 1)));
			}

			return nodeRepresentation;
		}
	}
}

