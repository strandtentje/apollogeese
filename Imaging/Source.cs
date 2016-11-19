using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.IO;

namespace Imaging
{
	public class Source : Service
	{
		public override string Description {
			get {
				return "File source";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["filename"] = defaultParameter;
		}

		string Filename {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.Filename = settings.GetString ("filename");
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction candidateOutgoing;

			if (parameters.TryGetClosest (typeof(IOutgoingBodiedInteraction), out candidateOutgoing)) {
				IOutgoingBodiedInteraction outgoing = (IOutgoingBodiedInteraction)candidateOutgoing;

				using(FileStream source = File.OpenRead(this.Filename)) {
					source.CopyTo (outgoing.OutgoingBody);
				}
			}

			return true;
		}
	}
}

