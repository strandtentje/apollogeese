using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace Data
{
	public abstract class DecoderService : Service
	{
		public bool TryGetDatareader(IInteraction parameters, IInteraction until, out TextReader reader) {
			IInteraction candidate;
			bool success = false;

			// But if a new incoming body was acquired after that
			if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), until, out candidate)) {
				// produce an XmlNodeInteraction from that instead.
				IIncomingBodiedInteraction source = (IIncomingBodiedInteraction)candidate;

				reader = source.GetIncomingBodyReader ();
				success = true;
			} else {
				reader = null;
			}

			return success;
		}
	}
}

