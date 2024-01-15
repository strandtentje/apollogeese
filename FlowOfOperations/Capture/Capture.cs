using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class Capture : Service
    {
        private string ScopeName;
        private Service InBranch;
        private Service OutBranch;

        public override string Description => "Captures and Stacks Interactions";
        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["scope"] = defaultParameter;
        }
        protected override void Initialize(Settings settings)
        {
            base.Initialize(settings);
            this.ScopeName = settings.GetString("scope", "defaultscope");
        }
        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name == "in") InBranch = e.NewValue;
            else if (e.Name == "out") OutBranch = e.NewValue;
            base.HandleBranchChanged(sender, e);
        }
        protected override bool Process(IInteraction parameters)
        {
            var cap = new CaptureInteraction(parameters, ScopeName);
            return InBranch.TryProcess(cap) && OutBranch.TryProcess(cap);
        }
    }
}
