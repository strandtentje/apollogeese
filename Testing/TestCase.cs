using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	public class TestCase : Service
	{
		public override string Description {
			get {
				return string.Format("Testing apparatus for case {0}", this.Name);
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name { get { return (string)this.Settings.Get ("default", this.Settings.GetString("name")); } }

		/// <summary>
		/// Gets the sourcename that should be used for incoming data interactions
		/// that are fed to the test subject
		/// </summary>
		/// <value>The name of the source.</value>
		string SourceName { get { return (string)this.Settings.Get ("sourcename", "http-request-body"); } }

		/// <summary>
		/// Gets or sets the available context.
		/// </summary>
		/// <value>The available context.</value>
		Settings AvailableContext { get { return this.Settings.GetSubsettings ("availablecontext"); } }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has ingoing data.
		/// </summary>
		/// <value><c>true</c> if this instance has ingoing data; otherwise, <c>false</c>.</value>
		bool HasIngoingData { get { return this.Settings.Has ("ingoingfile"); } }

		/// <summary>
		/// Gets or sets the ingoing data file.
		/// </summary>
		/// <value>The ingoing data file.</value>
		string IngoingDataFile { get { return this.Settings.GetString ("ingoingfile"); } }
				
		/// <summary>
		/// Gets or sets the test subject.
		/// </summary>
		/// <value>The test subject.</value>
		Service TestSubject { get { return Branches ["subject"]; } }

		/// <summary>
		/// Gets or sets the outgoing diff viewer.
		/// </summary>
		/// <value>The outgoing diff viewer.</value>
		Service OutgoingDataViewer { get { return Branches ["viewoutgoingdata"]; } }
		
		bool HasOutgoingDataViewer { get { return (this.Branches ["viewoutgoingdata"] ?? Stub) != Stub; } }

		/// <summary>
		/// Gets or sets the probe result viewer.
		/// </summary>
		/// <value>The probe result viewer.</value>
		Service MatchingProbeResultViewer { get { return Branches ["viewmatchingproberesult"]; } }

		/// <summary>
		/// Gets or sets the mismatching probe result viewer.
		/// </summary>
		/// <value>The mismatching probe result viewer.</value>
		Service MismatchingProbeResultViewer { get { return Branches ["viewmismatchingproberesult"]; } }

		protected override bool Process (IInteraction parameters)
		{
			TestContextInteraction testContext = null;
			IncomingTestData testInput = null;
			OutgoingTestableData testOutput = null;
			IInteraction testBundle;
			bool success;

			testBundle = testContext = new TestContextInteraction (this.Name, this.AvailableContext);

			if (HasIngoingData)
				testBundle = testInput = new IncomingTestData (this.IngoingDataFile, testBundle, this.SourceName);

			if (HasOutgoingDataViewer)
				testBundle = testOutput = new OutgoingTestableData (testBundle);

			success = this.TestSubject.TryProcess (testBundle);

			if (HasIngoingData)
				testInput.Dispose ();

			if (HasOutgoingDataViewer) {
				// obscure name of the year award goes to
				IInteraction incomingInteractionWithOutgoingData;
				incomingInteractionWithOutgoingData = new QuickIncomingInteraction (testOutput.GetProduct (), parameters, "test-case-output");

				success &= this.OutgoingDataViewer.TryProcess (incomingInteractionWithOutgoingData);

				testOutput.Dispose ();
			}

			foreach (ProbeResultInteraction result in testContext.ProbeResults) {
				if (result.IsMatch) 
					success &= this.MatchingProbeResultViewer.TryProcess (result);
				else 
					success &= this.MismatchingProbeResultViewer.TryProcess (result);
			}

			return success;
		}
	}
}

