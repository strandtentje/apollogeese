using System;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Text;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class SinkService : TwoBranchedService
	{
		private Service SourcingBranch = null;

		private Encoding FallbackEncoding;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.FallbackEncoding = Encoding.GetEncoding(
				settings.GetString(
					"fallbackencoding", 
					"utf-8"
				)
			);
		}

		protected delegate void ReadCallback(StreamReader reader);

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "source")
				this.SourcingBranch = e.NewValue;
		}

		protected void ReadFromParameters(IInteraction parameters, ReadCallback callback)
		{
			IInteraction sourceInteraction;

			if (parameters.TryGetClosest (
				    typeof(IIncomingReaderInteraction), 
				    out sourceInteraction
			    )) {
				callback (((IIncomingReaderInteraction)sourceInteraction).GetIncomingBodyReader ());
			} else {
				throw new Exception ("No IncomingReader available");
			}
		}

		protected void StartReading(IInteraction parameters, ReadCallback callback)
		{
			if (SourcingBranch == null) {
				ReadFromParameters (parameters, callback);
			} else {
				using (MemoryStream workingStream = new MemoryStream ()) {
					SimpleOutgoingInteraction sourceInteraction;
					sourceInteraction = new SimpleOutgoingInteraction (
                         workingStream, this.FallbackEncoding, parameters
					);

					if (this.SourcingBranch.TryProcess (sourceInteraction)) {
						if (sourceInteraction.HasWriter ())
							sourceInteraction.GetOutgoingBodyWriter ().Flush ();
						sourceInteraction.OutgoingBody.Position = 0;

						using (StreamReader workingReader = new StreamReader (workingStream)) {
							callback (workingReader);
						}
					} else {
						throw new Exception ("Failed to process source-branch");
					}
				}
			}
		}

		protected Encoding GetEncoding(IInteraction parameters)
		{
			if (SourcingBranch == null) {
				TextReader reader = ReadFromParameters (parameters).GetIncomingBodyReader ();
				if (reader is StreamReader) {
					return ((StreamReader)reader).CurrentEncoding;
				}
			} 

			return this.FallbackEncoding;
		}
	}
}

