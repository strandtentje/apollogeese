using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class Repeat : SingleBranchService
    {
        public override string Description => "Repeat n times";

        public string ForVariable { get; private set; }
        public string LoopVariable { get; private set; }
        public Service Looper { get; private set; }
        public Service Ender { get; private set; }

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

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            base.HandleBranchChanged(sender, e);
            if (e.Name == "loop") Looper = e.NewValue;
            if (e.Name == "_with") Looper = e.NewValue;
            if (e.Name == "end") Ender = e.NewValue;
        }

        protected override bool Process(IInteraction parameters)
        {
            bool success = true;
            if (parameters.TryGetFallback(ForVariable, out object forVar))
                for (int i = 0; i < Convert.ToInt32(forVar); i++)                
                    success &= Looper.TryProcess(
                        new SimpleInteraction(
                            parameters, 
                            LoopVariable, 
                            i));
            else
                return false;
            if (success) success &= Ender.TryProcess(parameters);
            return success;
        }
    }
}
