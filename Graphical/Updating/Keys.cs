using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graphical
{
    class Keys : Service
    {
        public override string Description
        {
            get { return "Keyboard handler"; }
        }

        List<Delta> Deltas = new List<Delta>();

        protected override void Initialize(Settings modSettings)
        {
            foreach(KeyValuePair<string, object> pair in modSettings.Dictionary) {
                if (pair.Value is Settings)
                {
                    Settings deltaConfig = (Settings)pair.Value;
                    Delta newDelta = new Delta();
                    newDelta.BranchName = pair.Key;
                    newDelta.Key = (Key)Enum.Parse(typeof(Key), deltaConfig.GetString("key"));
                    if (deltaConfig.Has("p")) newDelta.P = deltaConfig.GetFloat("p");
                    if (deltaConfig.Has("q")) newDelta.Q = deltaConfig.GetFloat("q");
                    if (deltaConfig.Has("r")) newDelta.R = deltaConfig.GetFloat("r");
                    Deltas.Add(newDelta);
                }
            }
        }

        protected override bool Process(IInteraction parameters)
        {
            if (parameters is UpdateInteraction)
            {
                UpdateInteraction update = (UpdateInteraction)parameters;

                foreach (Delta delta in Deltas)
                {
                    if (update.HeldKeys.Contains(delta.Key) )
                    {
                        Service target = Branches[delta.BranchName];
                        if (target is I3DParameterized)
                        {
                            I3DParameterized targetParameters = (I3DParameterized)target;
                            targetParameters.SetParameters(delta.P, delta.Q, delta.R);
                        }
                    }
                }
            }

            return true;
        }
    }

    struct Delta
    {
        public string BranchName;
        public Key Key;
        public float P;
        public float Q;
        public float R;
    }
}
