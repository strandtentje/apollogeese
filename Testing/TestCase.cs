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
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the diff tool command.
		/// </summary>
		/// <value>The diff tool command.</value>
		string DiffToolCommand { get; set; }

		/// <summary>
		/// Gets or sets the available context.
		/// </summary>
		/// <value>The available context.</value>
		Settings AvailableContext { get; set; }

		/// <summary>
		/// Gets or sets the test subject.
		/// </summary>
		/// <value>The test subject.</value>
		Service TestSubject { get; set;	}

		/// <summary>
		/// Gets or sets the outgoing diff viewer.
		/// </summary>
		/// <value>The outgoing diff viewer.</value>
		Service OutgoingDiffViewer { get; set; }

		/// <summary>
		/// Gets or sets the probe result viewer.
		/// </summary>
		/// <value>The probe result viewer.</value>
		Service ProbeResultViewer { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has ingoing data.
		/// </summary>
		/// <value><c>true</c> if this instance has ingoing data; otherwise, <c>false</c>.</value>
		bool HasIngoingData { get; set; }

		/// <summary>
		/// Gets or sets the ingoing data file.
		/// </summary>
		/// <value>The ingoing data file.</value>
		string IngoingDataFile { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has outgoing test data.
		/// </summary>
		/// <value><c>true</c> if this instance has outgoing test data; otherwise, <c>false</c>.</value>
		bool HasOutgoingDataVerification { get; set; }

		/// <summary>
		/// Gets or sets the outgoing data verification file.
		/// </summary>
		/// <value>The outgoing data verification file.</value>
		string OutgoingDataVerificationFile { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			this.Name = modSettings.GetString ("name");
			this.AvailableContext = modSettings.GetSubsettings ("availablecontext");
			this.DiffToolCommand = modSettings.GetString ("difftoolcommand", "diff");

			this.HasIngoingData = modSettings.Has ("ingoingdatafile");
			this.HasOutgoingDataVerification = modSettings.Has ("outgoingtestfile");

			if (HasIngoingData)
				this.IngoingDataFile = (string)modSettings ["ingoingfile"];

			if (HasOutgoingDataVerification)
				this.OutgoingDataVerificationFile = (string)modSettings ["outverificationfile"];
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "subject") {
				this.TestSubject = e.NewValue;
			}

			if (e.Name == "viewoutgoingdiff") {
				this.OutgoingDiffViewer = e.NewValue;
			}

			if (e.Name == "viewproberesult") {
				this.ProbeResultViewer = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			TestContext testContext = new TestContext (this.Name, this.AvailableContext);
			IncomingTestData testInput;
			OutgoingDataVerification testOutput;

			if (HasIngoingData)
				testInput = new IncomingTestData (this.IngoingDataFile, testContext);

			if (HasOutgoingDataVerification)
				testOutput = new OutgoingDataVerification (testInput ?? testContext);

			IInteraction testBundle = testOutput ?? testInput ?? testContext;

			bool success = this.TestSubject.TryProcess (testBundle);
			bool outgoingMatch = true;

			if (HasIngoingData)
				testInput.Dispose ();

			if (HasOutgoingDataVerification) {
				FileDiffInteraction diffInteraction = new FileDiffInteraction (
					this.DiffToolCommand,
					this.OutgoingDataVerificationFile,
					testOutput.GetProduct ());

				success &= this.OutgoingDiffViewer.TryProcess (diffInteraction);

				diffInteraction.Dispose ();
				testOutput.Dispose ();
			}

			foreach (ProbeResult result in testContext.ProbeResults) {

			}
		}
	}
}

