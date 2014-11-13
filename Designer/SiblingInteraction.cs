using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace Designer
{
	public class SiblingInteraction : QuickInteraction, IOutgoingBodiedInteraction
	{
		StreamWriter outgoingBody;
		MemoryStream body;

		public SiblingInteraction (IInteraction parent) : base(parent)
		{
			body = new MemoryStream();
			outgoingBody = new StreamWriter(body);
		}

		public string GetFinished()
		{
			body.Position = 0;
			StreamReader reader = new StreamReader(body);
			this["siblings"] = reader.ReadToEnd();
			reader.Close();
			return this.GetString("siblings", "");
		}

		public StreamWriter OutgoingBody { get { return outgoingBody; } }
	}
}

