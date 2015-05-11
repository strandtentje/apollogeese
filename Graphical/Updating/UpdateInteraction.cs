using BorrehSoft.ApolloGeese.Duckling;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphical
{
    class UpdateInteraction : IFast
    {
        private double timeDelta;
        public HashSet<Key> HeldKeys;

        public UpdateInteraction(HashSet<Key> hashSet)
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
