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

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "source")
				this.SourcingBranch = e.NewValue;
		}

		protected IIncomingReaderInteraction FindIncomingReader(IInteraction parameters)
		{
			IInteraction sourceInteraction;

			if (parameters.TryGetClosest (
				    typeof(IIncomingReaderInteraction), 
				    out sourceInteraction
			    )) {
				return (IIncomingReaderInteraction)sourceInteraction;
			} else {
				throw new Exception ("No IncomingReader available");
			}
		}

		protected TextReader GetReader(IInteraction parameters)
		{
			if (SourcingBranch == null) {
				return FindIncomingReader (parameters).GetIncomingBodyReader ();
			} else {
				SimpleOutgoingInteraction 
				sourceInteraction = new SimpleOutgoingInteraction (
                    new MemoryStream (), 
					this.FallbackEncoding, 
					parameters
				);

				if (this.SourcingBranch.TryProcess (sourceInteraction)) {
					if (sourceInteraction.HasWriter ())
						sourceInteraction.GetOutgoingBodyWriter ().Flush ();
					sourceInteraction.OutgoingBody.Position = 0;

					return new StreamReader (sourceInteraction.OutgoingBody);
				} else {
					throw new Exception ("Failed to process source-branch");
				}
			}
		}

		protected Encoding GetEncoding(IInteraction parameters)
		{
			if (SourcingBranch == null) {
				TextReader reader = FindIncomingReader (parameters).GetIncomingBodyReader ();
				if (reader is StreamReader) {
					return ((StreamReader)reader).CurrentEncoding;
				}
			} 

			return this.FallbackEncoding;
		}
	}
}

