using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	/// <summary>
	/// this can't be good
	/// </summary>
	public class OutgoingIterator : QuickInteraction, IOutgoingBodiedInteraction
	{
		string name;
		MemoryStream body;

		public OutgoingIterator (IInteraction parent, string name) : base(parent)
		{
			this.name = name;
			body = new MemoryStream();

		}

		public string GetFinished()
		{
			if (HasWriter ())
				GetOutgoingBodyWriter ().Flush ();

			body.Position = 0;
			StreamReader reader = new StreamReader(body);
			this[name] = reader.ReadToEnd();
			reader.Close();
			return this.GetString(name, "");
		}

		public Stream OutgoingBody { 
			get { return body; } 
		}

		private StreamWriter writer = null;

		public StreamWriter GetOutgoingBodyWriter() 
		{
			if (writer == null)
				writer = new StreamWriter (body);

			return writer;
		}

		public bool HasWriter()
		{
			return writer != null;
		}
	}
}

