using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utilities.Collections.Settings;
using System.Text;

namespace ExternalData
{
	public abstract class ExternalDataService : Service
	{
		protected Encoding Encoding { get; private set; }

		protected override void Initialize (Settings settings)
		{
			this.Encoding = Encoding.GetEncoding (settings.GetString ("encoding", "utf-8"));
			this.IsMimetypeChecking = settings.GetBool ("checkmimetype", true);
			this.Variable = settings.GetString ("variable", "");
			base.Initialize (settings);
		}

		/// <summary>
		/// Gets a value indicating whether this instance    is mimetype checking.
		/// </summary>
		/// <value><c>true</c> if this instance is mimetype checking; otherwise, <c>false</c>.</value>
		public bool IsMimetypeChecking { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is variable sourcing.
		/// </summary>
		/// <value><c>true</c> if this instance is variable sourcing; otherwise, <c>false</c>.</value>
		public bool IsVariableSourcing {
			get { return (Variable?.Length ?? 0) > 0; }
		}
			
		/// <summary>
		/// Gets a value indicating whether this instance uses a branch for acquiring data
		/// instead of existing context
		/// </summary>
		/// <value><c>true</c> if this instance is forward sourcing; otherwise, <c>false</c>.</value>
		public bool IsForwardSourcing {
			get { return Branches.Has ("source"); }
		}

		/// <summary>
		/// Gets the variable name to be read
		/// </summary>
		/// <value>The variable.</value>
		public string Variable {
			get;
			set; 
		}

		/// <summary>
		/// Gets the branch to invoke for acquiring data
		/// </summary>
		/// <value>The forward source.</value>
		public Service ForwardSource {
			get { return Branches ["source"]; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is self sourcing; useful for finding
		/// out if reader needs to be closed.
		/// </summary>
		/// <value><c>true</c> if this instance is self sourcing; otherwise, <c>false</c>.</value>
		public bool IsSelfSourcing {
			get { return IsVariableSourcing || IsForwardSourcing; }
		}

		public virtual bool CheckMimetype (string mimeType)
		{
			return true;
		}

		public bool TryGetDatareader(IInteraction parameters, IInteraction until, out TextReader reader) {
			IInteraction candidate;
			bool success;

			if (IsForwardSourcing) {
				MemoryStream dataTarget = new MemoryStream ();
				SimpleOutgoingInteraction dataTargetInteraction;
				dataTargetInteraction = new SimpleOutgoingInteraction (
					dataTarget, this.Encoding, parameters);

				success = ForwardSource.TryProcess (dataTargetInteraction);

				dataTargetInteraction.Done ();
				dataTarget.Position = 0;

				reader = new StreamReader (dataTarget, this.Encoding);
			} else if (IsVariableSourcing) {
			
				string value;

				success = parameters.TryGetFallbackString (this.Variable, out value);
			
				if (success) {
					reader = new StringReader (value);
				} else {
					reader = null;
				}
			} else if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), until, out candidate)) {
				IIncomingBodiedInteraction source = (IIncomingBodiedInteraction)candidate;

				success = CheckMimetype (source.ContentType);
				reader = source.GetIncomingBodyReader ();
			} else {
				success = false;
				reader = null;
			}

			return success;
		}
	}
}

