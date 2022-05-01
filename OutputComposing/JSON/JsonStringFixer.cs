using System.IO;
using System.Web;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

public class JsonStringFixer : SingleBranchService
{
    public override string Description => "Make strings be json compatible";

    public override void LoadDefaultParameters(string defaultParameter)
    {
        base.LoadDefaultParameters(defaultParameter);
    }

    protected override void Initialize(Settings settings)
    {
        base.Initialize(settings);
    }

    protected override bool Process(IInteraction parameters)
    {

        using (var ms = new MemoryStream())
        {
            var ogb = new SimpleOutgoingInteraction(ms, parameters);
            if(WithBranch.TryProcess(ogb))
            {
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    var b = Closest<IOutgoingBodiedInteraction>.From(parameters);
                    var sw = new StreamWriter(b.OutgoingBody);
                    sw.Write(HttpUtility.JavaScriptStringEncode(sr.ReadToEnd()));
                }
                return true;
            }
        }

        return false;
    }
}