using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Extensions.BasicHttpServer;
using BorrehSoft.Utilities.Collections.Settings;

namespace BasicHttpServer
{
    public class CacheControl : SingleBranchService
    {
        public override string Description => "Control the cache lifetime for resources";

        public string CCSetting { get; private set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["setting"] = defaultParameter;
        }

        protected override void Initialize(Settings settings)
        {
            this.CCSetting = settings.GetString("setting");
        }

        protected override bool Process(IInteraction parameters)
        {
            Closest<HttpInteraction>.From(parameters).ResponseHeaders.Add("Cache-Control", this.CCSetting);
            return WithBranch.TryProcess(parameters);
        }
    }
}
