using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
    public class BeginCaptureInteraction : SimpleInteraction
    {
        public BeginCaptureInteraction(IInteraction parent, string scopeName) : base(parent)
        {
            this.ScopeName = scopeName;
        }
        public CaptureInteraction Target { get; private set; }
        public string ScopeName { get; }
        public static CaptureInteraction FindAndRegister(IInteraction incoming, string scopename)
        {
            Stack<IInteraction> captures = new Stack<IInteraction>();
            while (incoming is IInteraction interaction)
                if (interaction is BeginCaptureInteraction begin && begin.ScopeName == scopename)
                    return begin.Target.Include(captures);
                else
                {
                    captures.Push(interaction);
                    incoming = interaction.Parent;
                }
            throw new NoCaptureScopeException(scopename);
        }
        public static BeginCaptureInteraction From(IInteraction parameters, string scopeName)
        {
            var cap = new BeginCaptureInteraction(parameters, scopeName);
            cap.Target = CaptureInteraction.FindIn(parameters, scopeName);
            return cap;
        }
    }
}
