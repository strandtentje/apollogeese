using System;
using System.IO;
using System.Runtime.Serialization;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridService : Service
{
    public override string Description => "Sends using sendgrid";

    public Service HtmlBranch { get; private set; }
    public Service PlainBranch { get; private set; }

    public override void LoadDefaultParameters(string defaultParameter)
    {
        base.LoadDefaultParameters(defaultParameter);
    }

    protected override void Initialize(Settings settings)
    {
        base.Initialize(settings);
    }

    protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
    {
        if (e.Name == "html") HtmlBranch = e.NewValue;
        if (e.Name == "plain") PlainBranch = e.NewValue;
    }

    string Config(IInteraction parameters, string name)
    {
        if (Settings.TryGetString(name + "_override", out string varName))
            if (parameters.TryGetFallbackString(varName, out string val))
                return val;
            else
                throw new MissingMailInfoException(name);
        else if (Settings.TryGetString(name, out string val))
            return val;
        else
            throw new MissingMailInfoException(name);
    }

    protected override bool Process(IInteraction parameters)
    {
        var client = new SendGridClient(Config(parameters, "apikey"));
        var from = new EmailAddress(Config(parameters, "senderaddress"), Config(parameters, "sendername"));
        var to = new EmailAddress(Config(parameters, "toaddress"), Config(parameters, "toname"));
        var subject = Config(parameters, "subject");

        var succ = true;
        var htmlEmail = "";
        var plainEmail = "";

        using (var htmlMs = new MemoryStream()) {
            var htmlOutgoing = new SimpleOutgoingInteraction(htmlMs, parameters);
            succ &= (HtmlBranch ?? Stub).TryProcess(htmlOutgoing);
            htmlMs.Position = 0;
            using (var htmlSr = new StreamReader(htmlMs))
            {
                htmlEmail = htmlSr.ReadToEnd();
            }
        }

        using (var ptMs = new MemoryStream())
        {
            var ptOutgoing = new SimpleOutgoingInteraction(ptMs, parameters);
            succ &= (PlainBranch ?? Stub).TryProcess(ptOutgoing);
            ptMs.Position = 0;
            using (var ptSr = new StreamReader(ptMs))
            {
                plainEmail = ptSr.ReadToEnd();
            }
        }

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainEmail, htmlEmail);
        var task = client.SendEmailAsync(msg);
        task.Wait();

        return task.Result.IsSuccessStatusCode;
    }
}

internal class MissingMailInfoException : Exception
{
    public MissingMailInfoException(string name) : base(name)
    {
    }
}