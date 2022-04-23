using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.CoreTypes.Services.Simple
{
    public class Switch : Service
    {
        public override string Description => "Analog to a switch statement";

        public string VariableName { get; private set; }
        public Service DefaultBranch { get; private set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["variablename"] = defaultParameter;
        }

        protected override void Initialize(Settings settings)
        {
            this.VariableName = settings.GetString("variablename");
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            base.HandleBranchChanged(sender, e);
            if (e.Name == "default__")
            {
                this.DefaultBranch = e.NewValue;
            }
        }

        protected override bool Process(IInteraction parameters)
        {
            var switchable = Fallback<object>.From(parameters, this.VariableName);
            var strSwitch = switchable.ToString();

            if (Branches.Has(strSwitch))
            {
                return Branches[strSwitch].TryProcess(parameters);
            }
            else if (DefaultBranch is null)
            {
                return false;
            }
            else
            {
                return DefaultBranch.TryProcess(parameters);
            }
        }
    }
}
