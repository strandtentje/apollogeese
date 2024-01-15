using BorrehSoft.ApolloGeese.CoreTypes;
using System;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class CaptureInteraction : IInteraction
    {
        public CaptureInteraction(IInteraction parent, string scopeName)
        {
            this.Parent = parent;
            this.ScopeName = scopeName;
            this.ExceptionHandler = parent.ExceptionHandler;
        }
        public static CaptureInteraction FindIn(IInteraction incoming, string scopename)
        {
            while (incoming is IInteraction interaction)
                if (interaction is CaptureInteraction begin && begin.ScopeName == scopename)
                    return begin;
                else
                    incoming = interaction.Parent;
            throw new NoCaptureScopeException(scopename);
        }
        private List<Stack<IInteraction>> allCaptures = new List<Stack<IInteraction>>();

        public string ScopeName { get; }

        public IInteraction Root => Parent.Root;
        public IInteraction Parent { get; private set; }
        public ExceptionHandler ExceptionHandler { get; private set; } = SimpleInteraction.DefaultEHandler;
        public object this[string name]
        {
            get
            {
                if (TryGetValue(name, out object result))
                    return result;
                else 
                    return null;
            }
        }

        public CaptureInteraction Include(Stack<IInteraction> captures)
        {
            allCaptures.Add(captures);
            return this;
        }

        public IInteraction GetClosest(Type t) => TryGetClosest(t, out IInteraction interaction) ? interaction : throw new Exception($"No interaction in chain was of type");
        public bool TryGetClosest(Type t, out IInteraction closest) => TryGetClosest(t, null, out closest);
        public bool TryGetClosest(Type t, IInteraction limit, out IInteraction closest)
        {
            closest = this;
            while (closest is IInteraction result)
                if (result == limit)
                    return false;
                else if (t.IsInstanceOfType(result))
                    return true;
                else
                    closest = result.Parent;
            return false;
        }
        public IInteraction Clone(IInteraction parent) => new CaptureInteraction(parent, ScopeName) { allCaptures = this.allCaptures };
        public bool TryGetString(string id, out string luggage)
        {
            if (TryGetValue(id, out object maybe) && maybe is string s)
            {
                luggage = s;
                return true;
            } else
            {
                luggage = "";
                return false;
            }
        }

        public bool TryGetValue(string id, out object luggage)
        {
            foreach (var capture in allCaptures)
                foreach (var interaction in capture)
                    if (interaction.TryGetValue(id, out luggage))
                        return true;
            luggage = "";
            return false;
        }

        public bool TryGetFallbackString(string id, out string luggage)
        {
            if (TryGetFallback(id, out object maybe) && maybe is string s)
            {
                luggage = s;
                return true;
            } else
            {
                luggage = "";
                return false;
            }
        }

        public bool TryGetFallback(string id, out object luggage)
        {
            if (TryGetValue(id, out luggage))
                return true;
            else
                return Parent.TryGetFallback(id, out luggage);
        }
    }
}
