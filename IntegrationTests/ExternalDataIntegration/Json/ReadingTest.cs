using NUnit.Framework;
using System;
using BorrehSoft.ApolloGeese.Loader;
using System.IO;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using Probing;
using BorrehSoft.Utilities.Log;

namespace ExternalDataIntegration.Json
{
	[TestFixture ()]
	public class ReadingTest
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
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Json", "readingtest.conf"), 
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Json"),
				false, true
			);

			BaseInteraction = new SimpleInteraction ();
		}

		[TearDown ()]
		public void TearDown ()
		{
			Secretary.LatestLog.Dispose ();
		}

		private SimpleInteraction Probe (string name, int count = 1)
		{
			Service service = Services.Get (name);
			DumbProbe probe = new DumbProbe ();
			var branches = new Map<Service> ();
			branches ["mapped"] = probe;
			JumpInteraction interaction = new JumpInteraction (BaseInteraction, branches, new Map<string> (), new Map<object> ());
			Assert.True (service.TryProcess (interaction));
			Assert.AreEqual (count, probe.CallCounter);
			Assert.True (probe.LastInteraction is SimpleInteraction);
			return (SimpleInteraction)probe.LastInteraction;
		}

		[Test ()]
		public void EmptyTester ()
		{
			Assert.AreEqual (0, Probe ("empty").Dictionary.Count);
		}

		[Test ()]
		public void SimpleTester ()
		{
			Assert.AreEqual ("lekker", Probe ("simple").GetString ("kaas"));
		}

		[Test ()]
		public void NestedTester ()
		{
			Assert.AreEqual ("complex", Probe ("nested").GetString ("is"));
		}

		[Test ()]
		public void ArrayTester ()
		{
			Assert.AreEqual (27, Probe ("array", 4) ["kaas"]);
		}
	}
}

