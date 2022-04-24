using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class Repeat : SingleBranchService
    {
        public override string Description => "Repeat n times";

        public string ForVariable { get; private set; }
        public string LoopVariable { get; private set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            var ternary = defaultParameter.Split('>');
            if (ternary.Length == 2)
            {
                Settings["forvariable"] = ternary[0];
                Settings["loopvariable"] = ternary[1];
            }
        }

        protected override void Initialize(Settings settings)
        {
            ForVariable = settings.GetString("forvariable");
            LoopVariable = settings.GetString("loopvariable");
        }

        protected override bool Process(IInteraction parameters)
        {
            bool success = true;
            if (parameters.TryGetValue(ForVariable, out object forVar))
                for (int i = 0; i < Convert.ToInt32(forVar); i++)                
                    success &= WithBranch.TryProcess(
                        new SimpleInteraction(
                            parameters, 
                            LoopVariable, 
                            i));
            else
                return false;
            return success;
        }
    }
}
