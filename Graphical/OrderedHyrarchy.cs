using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphical
{
    abstract class OrderedHyrarchy : Service
    {
        List<Service> over = new List<Service>();
        List<Service> under = new List<Service>();
        
        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name.StartsWith("over"))
            {
                if (e.PreviousValue != null) over.Remove(e.PreviousValue);
                if (e.NewValue != null) over.Add(e.NewValue);
            }
            else if (e.Name.StartsWith("under"))
            {
                if (e.PreviousValue != null) under.Remove(e.PreviousValue);
                if (e.NewValue != null) under.Add(e.NewValue);
            }
        }

        public abstract bool CentralProcess(IFast parameters);

        public virtual bool Finalizer(IFast parameters)
        {
            return true;
        }

        public override bool FastProcess(IFast parameters)
        {
            bool result = true;

            foreach (Service child in under)
                result &= child.FastProcess(parameters);

            result &= CentralProcess(parameters);

            foreach (Service child in over)
                result &= child.FastProcess(parameters);

            result &= Finalizer(parameters);

            return result;
        }
    }
}
