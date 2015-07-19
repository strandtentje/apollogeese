using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	/// <summary>
	/// Reads data from IncomingInteraction into one buffer until a splitter sequence is detected, then swaps buffer so data may be read.
	/// </summary>
	public class DoubleBufferedSplitter : Service
	{
		byte[] delimiter;
		object accessingIncoming = new object();
		object accessingOutgoing = new object();
		Stream incoming;
		Timer splitterThread;
		MimeType mimeType;

		int bufsize = 500000, interval = 100;

		MemoryStream 
			TargetBuffer = new MemoryStream(), 
			SourceBuffer = new MemoryStream();
		
		public override string Description {
			get {
				return "Splits from last available IncomingInteraction";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			IEnumerable<object> delimiterSetting = (IEnumerable<object>)modSettings ["delimiter"];
			List<byte> delimiterByteList = new List<byte> ();

			foreach (int listByte in delimiterSetting)
				delimiterByteList.Add ((byte)listByte);

			delimiter = delimiterByteList.ToArray ();

			mimeType = MimeType.FromString(modSettings.GetString("mimetype", "application/octet-stream"));

			if (modSettings.Has ("bufsize"))
				bufsize = (int)modSettings ["bufsize"];
			if (modSettings.Has ("interval"))
				interval = (int)modSettings ["interval"];

			buffer = new byte[bufsize];
		}

		/// <summary>
		/// Temporarily locks onto r/w ops and swaps the buffers.
		/// </summary>
		void SwapBuffers ()
		{
			lock (accessingOutgoing) {
				MemoryStream AlterBuffer;

				AlterBuffer = SourceBuffer;
				SourceBuffer = TargetBuffer;
				TargetBuffer = AlterBuffer;

				SourceBuffer.Position = 0;
				TargetBuffer.Position = 0;
			}
		}
		
		byte[] buffer;
		int length; int delimiterIndex = 0;

		/// <summary>
		/// Copies from Incoming to Outgoing until a splitter character
		/// is detechted.
		/// </summary>
		/// <param name="p">We don't know what this guy does but I'm
		/// anxious to take it away.</param>
		void Splitter(object p)
		{
			if (!incoming.CanRead) {
				splitterThread = null;
				splitterThread.Dispose ();
			}

			lock (accessingIncoming) length = incoming.Read (buffer, 0, buffer.Length);

			for (int index = 0; index < length; index++) {
				if (delimiterIndex >= delimiter.Length) {
					SwapBuffers ();
					TargetBuffer.Write (delimiter, 0, delimiterIndex);
					TargetBuffer.Write (buffer, index, 1);
					delimiterIndex = 0;
				} else if (buffer [index] == delimiter [delimiterIndex]) {
					delimiterIndex++;
				} else {
					TargetBuffer.Write (delimiter, 0, delimiterIndex);
					TargetBuffer.Write (buffer, index, 1);
					delimiterIndex = 0;
				}
			}
		}

		/// <summary>
		/// Makes incoming buffer when needed and then periodically checks if 
		/// it can be filled or should be finished.
		/// </summary>
		/// <param name="incomingBody">Incoming body.</param>
		void BufferAndSplit (Stream incomingBody)
		{
			if (incoming != incomingBody) {
				lock (accessingIncoming)
					incoming = incomingBody;

				if (splitterThread == null) {
					splitterThread = new Timer (Splitter, null, 10, interval);
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IIncomingBodiedInteraction incomingData;
			IOutgoingBodiedInteraction outgoingData;

			incomingData = (IIncomingBodiedInteraction)parameters.GetClosest (typeof(IIncomingBodiedInteraction));
			outgoingData = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));

			if (outgoingData is IHttpInteraction) 	
				((IHttpInteraction)outgoingData).ResponseHeaders.ContentType = mimeType;

			BufferAndSplit (incomingData.IncomingBody);

			lock(accessingOutgoing)
				SourceBuffer.CopyTo (outgoingData.OutgoingBody);

			return true;
		}
	}
}

