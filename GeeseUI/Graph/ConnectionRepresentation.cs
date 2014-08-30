using System;
using felinet.Reality.Physical.Visual;

namespace GeeseUI.Graph
{
	public class ConnectionRepresentation : Representation
	{
		public NodeRepresentation Pat { get; private set; }

		public NodeRepresentation Mat { get; private set; }

		public ConnectionRepresentation(NodeRepresentation Pat, NodeRepresentation Mat)
		{
			this.Pat = Pat;
			this.Mat = Mat;
		}
	}
}

