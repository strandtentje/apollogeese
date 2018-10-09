using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Log;
using NUnit.Framework;
using Probing;

namespace AuthIntergration
{
	[TestFixture()]
    public class NonceTest
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
                Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Json", "noncetest.conf"), 
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
            branches ["_with"] = probe;
            var interaction = new JumpInteraction (
				BaseInteraction, branches, 
				new Map<string> (), new Map<object> ()
		    );
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
        
    }
}
