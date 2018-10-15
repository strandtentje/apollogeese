using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Log;
using Networking;
using NUnit.Framework;
using Probing;
using System;
using System.IO;

namespace NetworkingTests
{
    [TestFixture()]
    public class Test
    {
		string dodgyRussianProxy = "193.138.223.46:3128";

		SimpleInteraction BaseInteraction;

        ServiceCollection Services {
            get;
            set;
        }

        [SetUp ()]
        public void SetUp ()
        {
            (new Secretary ("integration")).ReportHere (5, "open!");

            Services = ServiceCollectionCache.Get (
                Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "proxytest.conf"), 
                AppDomain.CurrentDomain.BaseDirectory,
                false, true
            );

    

            BaseInteraction = new SimpleInteraction ();
        }

        [TearDown ()]
        public void TearDown ()
        {
            Secretary.LatestLog.Dispose ();
        }

        [Test ()]
		public void ProxyTest ()
        {
			var service = this.Services.Get("proxytest");
			var goodprobe = new DumbProbe ();
			var badprobe = new DumbProbe();
            var branches = new Map<Service> ();
            branches ["success"] = goodprobe;
			branches ["fail"] = badprobe;
			var injvar = new Map<object> ();
			injvar["proxyserver"] = dodgyRussianProxy;
            var interaction = new JumpInteraction (BaseInteraction, branches, new Map<string> (), injvar);
            Assert.True (service.TryProcess (interaction));
			Assert.AreEqual (1, goodprobe.CallCounter);
			Assert.AreEqual(0, badprobe.CallCounter);
			var respin = goodprobe.LastInteraction as SimpleInteraction;
			Assert.NotNull(respin);
			string result;
			if (respin.TryGetFallbackString("httpresult", out result)) {
				Assert.True(result.Contains("<html"));
			} else {
				Assert.Fail("no httpresult");
			}
        }
    }
}
