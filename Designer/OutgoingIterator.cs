using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace Designer
{
	public class OutgoingIterator : QuickInteraction, IOutgoingBodiedInteraction
	{
		string name;
		StreamWriter outgoingBody;
		MemoryStream body;

		public OutgoingIterator (IInteraction parent, string name) : base(parent)
		{
			this.name = name;
			body = new MemoryStream();
			outgoingBody = new StreamWriter(body);
		}

		public string GetFinished()
		{
			outgoingBody.Flush();
			body.Position = 0;
			StreamReader reader = new StreamReader(body);
			this[name] = reader.ReadToEnd();
			reader.Close();
			return this.GetString(name, "");
		}

		public StreamWriter OutgoingBody { get { return outgoingBody; } }
	}
}

