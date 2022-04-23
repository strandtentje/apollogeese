using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

public class SendGrid : Service
{
    public override string Description => "Sends using sendgrid";

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
        return base.Process(parameters);
    }
}