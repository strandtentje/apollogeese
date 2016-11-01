using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class StringProcessorInteraction : SimpleOutgoingInteraction
	{
		MemoryStream UnderlyingStream;
		IOutgoingBodiedInteraction SinkInteraction;

		private StringProcessorInteraction (
			MemoryStream underlyingStream,
			IInteraction parent,
			IOutgoingBodiedInteraction sinkInteraction
		) : base(
			underlyingStream, 
			sinkInteraction.Encoding,
			parent
		) {
			this.UnderlyingStream = underlyingStream;
			this.SinkInteraction = sinkInteraction;
		}

		public static StringProcessorInteraction From(IInteraction parent) {
			return new StringProcessorInteraction (
				new MemoryStream(),
				parent, 
				Closest<IOutgoingBodiedInteraction>.From (parent)
			);
		}

		public void Run(Func<string, string> processor) 
		{
			Done ();
			UnderlyingStream.Position = 0;

			byte[] data;

			using (StreamReader reader = new StreamReader (UnderlyingStream))
				data = Encoding.GetBytes (processor (reader.ReadToEnd ()));
				
			SinkInteraction.OutgoingBody.Write (data, 0, data.Length);
		}
	}
}

