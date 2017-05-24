using NUnit.Framework;
using System;
using BorrehSoft.ApolloGeese.Loader;
using System.IO;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using Probing;
using BorrehSoft.Utilities.Log;

namespace ExternalDataIntegration.Date
{
	[TestFixture ()]
	public class DateTolleranceTest
	{
		ServiceCollection Services {
			get;
			set;
		}

		SimpleInteraction BaseInteraction;

		[SetUp ()]
		public void SetUp ()
		{
			(new Secretary ("integration")).ReportHere (5, "open!");
			
			Services = ServiceCollectionCache.Get (
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Date", "datetest.conf"), 
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Date"),
				false, true
			);

			BaseInteraction = new SimpleInteraction ();
		}

		[TearDown ()]
		public void TearDown ()
		{
			Secretary.LatestLog.Dispose ();
		}

		private void Probe (string name, int equalCount = 1, int notEqualCount = 0)
		{
			Service service = Services.Get (name);
			DumbProbe equalProbe = new DumbProbe (), notequalProbe = new DumbProbe();
			var branches = new Map<Service> ();
			branches ["equal"] = equalProbe; branches["notequal"] = notequalProbe;
			JumpInteraction interaction = new JumpInteraction (BaseInteraction, branches, new Map<string> (), new Map<object> ());
			Assert.True (service.TryProcess (interaction));
			Assert.AreEqual (equalCount, equalProbe.CallCounter);
			Assert.AreEqual(notEqualCount, notequalProbe.CallCounter);
			Assert.True ((equalProbe.LastInteraction ?? notequalProbe.LastInteraction) is SimpleInteraction);
		}

		[Test ()]
		public void ExactTester ()
		{
			Probe("exact");
		}

		[Test ()]
		public void SimilarTester ()
		{
			Probe("similar");
		}

		[Test ()]
		public void EdgeTester ()
		{
			Probe("edge");
		}

		[Test ()]
		public void MismatchTester ()
		{
			Probe("out", 0, 1);
		}

		[Test ()]
		public void BigMismatchTester ()
		{
			Probe("wayout", 0, 1);
		}
	}
}

