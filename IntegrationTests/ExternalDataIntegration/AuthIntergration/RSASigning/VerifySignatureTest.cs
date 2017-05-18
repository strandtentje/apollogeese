using NUnit.Framework;
using System;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utilities.Log;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using Probing;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;

namespace AuthIntergration
{
	[TestFixture ()]
	public class VerifySignatureTest
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
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "RSASigning", "verifyingtest.conf"), 
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "RSASigning"),
				false, true
			);

			BaseInteraction = new SimpleInteraction ();
		}

		[TearDown ()]
		public void TearDown ()
		{
			Secretary.LatestLog.Dispose ();
		}

		private void Probe (string name, int successfulCount, int failureCount)
		{
			Service service = Services.Get (name);
			DumbProbe successfulProbe = new DumbProbe (), failureProbe = new DumbProbe();
			var branches = new Map<Service> ();
			branches ["successful"] = successfulProbe;
			branches["failure"] = failureProbe;
			JumpInteraction interaction = new JumpInteraction (BaseInteraction, branches, new Map<string> (), new Map<object> ());
			Assert.True (service.TryProcess (interaction));
			Assert.AreEqual (successfulCount, successfulProbe.CallCounter);
			Assert.AreEqual (failureCount, failureProbe.CallCounter);
		}

		[Test ()]
		public void TestCorrectSignature ()
		{
			Probe("valid", 1, 0);
		}

		[Test()]
		public void TestIncorrectSignature() 
		{
			Probe("invalid", 0, 1);
		}
	}
}

