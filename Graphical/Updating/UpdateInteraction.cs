using BorrehSoft.ApolloGeese.CoreTypes;

using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graphical
{
    class UpdateInteraction : IFast
    {
        private double timeDelta;
        public List<Key> HeldKeys;

        public UpdateInteraction(List<Key> hashSet)
        {
            // TODO: Complete member initialization
            this.HeldKeys = hashSet;
        }

        internal double GetTimedelta()
        {
            return timeDelta;
        }

        internal void SetTimedelta(double time)
        {
            this.timeDelta = time;
        }
    }
}
