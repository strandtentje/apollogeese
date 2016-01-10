using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text;

namespace ExternalData
{
	public abstract class ExternalDataService : Service
	{
		Encoding Encoding;

		protected override void Initialize (Settings settings)
		{
			this.Encoding = Encoding.GetEncoding (settings.GetString ("encoding", "utf-8"));
			base.Initialize (settings);
		}

		public bool TryGetDatareader(IInteraction parameters, IInteraction until, out TextReader reader) {
			IInteraction candidate;
			bool success;

			if (Branches.Has ("source")) {
				MemoryStream dataTarget = new MemoryStream ();
				SimpleOutgoingInteraction dataTargetInteraction;
				dataTargetInteraction = new SimpleOutgoingInteraction(
					dataTarget, this.Encoding, parameters);

				success = Branches ["source"].TryProcess (dataTargetInteraction);

				dataTargetInteraction.Done();
				dataTarget.Position = 0;

				reader = new StreamReader(dataTarget, this.Encoding);
			} else if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), until, out candidate)) {
				IIncomingBodiedInteraction source = (IIncomingBodiedInteraction)candidate;

				success = true;
				reader = source.GetIncomingBodyReader ();
			} else {
				success = false;
				reader = null;
			}

			return success;
		}
	}
}

