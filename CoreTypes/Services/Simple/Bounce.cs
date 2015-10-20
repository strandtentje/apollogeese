using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
    class Bounce : Service
    {
        public override string Description
        {
            get { return "Incoming data becomes outgoing data."; }
        }

        protected override void Initialize(Settings modSettings)
        {
            
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {

        }

        protected override bool Process(IInteraction parameters)
        {
            IIncomingBodiedInteraction incoming = (IIncomingBodiedInteraction)parameters.GetClosest(typeof(IIncomingBodiedInteraction));
            IOutgoingBodiedInteraction outgoing = (IOutgoingBodiedInteraction)parameters.GetClosest(typeof(IOutgoingBodiedInteraction));
            
            incoming.IncomingBody.CopyTo(outgoing.OutgoingBody);

            return true;
        }
    }
}
