using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	/// <summary>
	/// this can't be good
	/// </summary>
	public class OutgoingIterator : SimpleInteraction, IOutgoingBodiedInteraction
	{
		string name;
		MemoryStream body;

		public OutgoingIterator (IInteraction parent, string name) : base(parent)
		{
			this.name = name;
			this.Encoding = Encoding.UTF8;
			body = new MemoryStream();
		}

		public Encoding Encoding { 
			get;
			private set;
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
				writer = new StreamWriter (body, this.Encoding);

			return writer;
		}

		public bool HasWriter()
		{
			return writer != null;
		}
	}
}

