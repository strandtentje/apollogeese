using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
    public class MarkdownToHtml : Service
    {
        public override string Description => "Converts marktdown to html";

        public string SourceVariable { get; private set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            Settings["variable"] = defaultParameter;
        }

        protected override void Initialize(Settings settings)
        {
            this.SourceVariable = settings.GetString("variable", "markdown");
        }

        protected override bool Process(IInteraction parameters)
        {
            var incomingMarkdown = Fallback<string>.From(parameters, SourceVariable);
            var outgoingHtml = CommonMark.CommonMarkConverter.Convert(incomingMarkdown);
            var outgoingHtmlBytes = Encoding.UTF8.GetBytes(outgoingHtml);
            Closest<IOutgoingBodiedInteraction>.From(
                parameters).OutgoingBody.Write(
                outgoingHtmlBytes, 0, outgoingHtmlBytes.Length);

            return true;
        }
    }
}
