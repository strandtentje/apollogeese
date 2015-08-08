using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Diagnostics;
using DProcess = System.Diagnostics.Process;
using System.IO;
using BorrehSoft.Utensils.Collections;

namespace Testing.Diff
{
	public class DiffFinder : Service
	{		
		public override string Description {
			get {
				return "Diffs file with incoming data";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["verificationfile"] = defaultParameter;
		}

		/// <summary>
		/// Gets or sets the diff tool command.
		/// </summary>
		/// <value>The diff tool command.</value>
		string DiffToolCommand { get { return this.Settings.GetString ("difftoolcommand", "diff"); } }

		/// <summary>
		/// Gets or sets the outgoing data verification file.
		/// </summary>
		/// <value>The outgoing data verification file.</value>
		string VerificationFile { get { return this.Settings.GetString("verificationfile"); } }

		Service Summary { get { return this.Branches ["summary"] ?? Stub; } }

		Service DiffLine { get { return this.Branches ["difference"] ?? Stub; } }

		Service PerfectMatch { get { return this.Branches ["perfect"] ?? Stub; } }

		/// <summary>
		/// Gets the diff tool start info.
		/// </summary>
		/// <returns>The diff tool start info.</returns>
		ProcessStartInfo GetDiffToolStartInfo ()
		{
			return new ProcessStartInfo (
				this.DiffToolCommand, string.Format ("-u {0} -", this.VerificationFile)) {
				UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true
			};
		}

		/// <summary>
		/// Distributes the diff.
		/// </summary>
		/// <returns><c>true</c>, if diff was distributed, <c>false</c> otherwise.</returns>
		/// <param name="diffStream">Diff stream.</param>
		/// <param name="parameters">Parameters.</param>
		bool DistributeDiff (StreamReader diffStream, IInteraction parameters)
		{
			bool result = true;

			if (diffStream.EndOfStream) {
				result &= PerfectMatch.TryProcess (parameters);
			} else {
				Map<object> summary = new Map<object> ();
				summary ["leftfile"] = diffStream.ReadLine ();
				summary ["rightfile"] = diffStream.ReadLine ();
				summary ["changecounter"] = diffStream.ReadLine ();

				result &= this.Summary.TryProcess (new QuickInteraction (parameters, summary));

				while (!diffStream.EndOfStream) {
					result &= this.DiffLine.TryProcess (new DifferenceInteraction (diffStream.ReadLine (), parameters));
				}
			}

			return result;
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction unparsed;
			bool result = true;

			if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), out unparsed)) {				
				IIncomingBodiedInteraction incomingData;
				Process diffToolProcess;

				incomingData = (IIncomingBodiedInteraction)unparsed;

				diffToolProcess = DProcess.Start (GetDiffToolStartInfo());

				incomingData.IncomingBody.CopyTo (diffToolProcess.StandardInput.BaseStream);
				diffToolProcess.StandardInput.Close ();

				DistributeDiff (diffToolProcess.StandardOutput, parameters);

				diffToolProcess.Dispose ();
			} else {
				throw new Exception ("No incoming stream");
			}

			return result;
		}
	}
}

