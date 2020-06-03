using System;
using System.Threading;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace Testing
{
    public class WaitTime : SingleBranchService
    {
        public override string Description => "Wait some specified amount of time";

        public int Timeout { get; private set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["timeout"] = int.Parse(defaultParameter);
        }

        protected override void Initialize(Settings settings)
        {
            this.Timeout = settings.GetInt("timeout", 1000);
        }

        protected override bool Process(IInteraction parameters)
        {
            Thread.Sleep(this.Timeout);
            return WithBranch.TryProcess(parameters);
        }
    }
}
