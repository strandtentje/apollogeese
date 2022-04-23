using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class Switch : SingleBranchService
    {
        public string SwitchVar { get; private set; }

        public override string Description => "the switch statement. dont use this";

        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["switchvariable"] = defaultParameter;
        }

        protected override void Initialize(Settings settings)
        {
            this.SwitchVar = settings.GetString("switchvariable");
        }

        protected override bool Process(IInteraction parameters)
        {
            return parameters.TryGetFallbackString(
                    this.SwitchVar,
                    out string switchvalue) && 
                    Branches.Has(switchvalue) && 
                    Branches[switchvalue].TryProcess(parameters);
        }
    }
}
