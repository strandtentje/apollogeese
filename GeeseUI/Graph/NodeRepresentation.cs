using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Duckling;
using felinet.Reality.Physical.Visual;

namespace GeeseUI.Graph
{
	public class NodeRepresentation : Representation
	{
		public WatchableMap<object> Configuration;
		public int Depth { get; private set; }

		public NodeRepresentation (Service Node, int Depth)
		{
			this.Configuration = new WatchableMap<object>(Node.GetSettings());
			this.Depth = Depth;
		}
	}
}

