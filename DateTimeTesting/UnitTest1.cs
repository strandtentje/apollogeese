using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using ExternalData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DateTimeTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var split = new SplitDateTime();
            var merge = new MergeDateTime();
            var splitSettings = new Settings();
            var mergeSettings = new Settings();
            var splitBranches = new WatchableMap<Service>();
            var mergeBranches = new WatchableMap<Service>();
            var testService = new TestService();
            splitSettings["default"] = "datetimein>dateinter,timeinter";
            mergeSettings["default"] = "dateinter,timeinter>datetimeout";
            split.SetBranches(splitBranches);
            merge.SetBranches(mergeBranches);
            split.SetSettings(splitSettings);
            merge.SetSettings(mergeSettings);
            splitBranches["_with"] = merge;
            mergeBranches["_with"] = testService;
            DateTime given = DateTime.Now;
            string givenIsoString = given.ToString("o");
            var interaction = new SimpleInteraction(new SimpleInteraction(), "datetimein", givenIsoString);
            Assert.IsTrue(split.TryProcess(interaction));
            Assert.IsTrue(testService.Received.TryGetFallbackString("datetimeout", out string resultingIsoString));
            DateTime resulting = DateTime.Parse(resultingIsoString, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var diff = given - resulting;
            Assert.IsTrue(Math.Abs(diff.TotalSeconds) < 60);
        }
    }

    internal class TestService : Service
    {
        public override string Description => "Service for testing";

        public IInteraction Received { get; private set; }

        protected override bool Process(IInteraction parameters)
        {
            this.Received = parameters;
            return true;
        }
    }
}
