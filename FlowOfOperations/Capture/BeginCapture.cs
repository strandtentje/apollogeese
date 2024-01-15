using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class BeginCapture : SingleBranchService
    {
        private string ScopeName;
        public override string Description => "Begin capturing within Capture scope";
        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["scope"] = defaultParameter;
        }
        protected override void Initialize(Settings settings)
        {
            base.Initialize(settings);
            this.ScopeName = settings.GetString("scope", "defaultscope");
        }
        protected override bool Process(IInteraction parameters)
        {
            return WithBranch.TryProcess(BeginCaptureInteraction.From(parameters, ScopeName));
        }
    }
}
