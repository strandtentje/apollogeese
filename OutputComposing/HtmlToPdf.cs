using System.IO;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using TheArtOfDev.HtmlRenderer.PdfSharp;

public class HtmlToPdf : SingleBranchService
{
    public override string Description => "Convert outgoing data from html to pdf";

    protected override void Initialize(Settings settings)
    {
        base.Initialize(settings);
    }

    protected override bool Process(IInteraction parameters)
    {
        using (var ms = new MemoryStream())
        {
            var og = new SimpleOutgoingInteraction(ms, Encoding.UTF8, parameters);
            if (WithBranch.TryProcess(og))
            {
                var sr = new StreamReader(ms);
                var pdf = PdfGenerator.GeneratePdf(sr.ReadToEnd(), PdfSharp.PageSize.Letter);

                var op = Closest<IOutgoingBodiedInteraction>.From(parameters);
                pdf.Save(op.OutgoingBody);
                return true;
            }
        }

        return false;
    }
}