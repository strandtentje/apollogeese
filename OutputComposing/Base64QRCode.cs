using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
    public class Base64QRCode : SingleBranchService
    {
        public override string Description => "Generates a QR code";

        public string SourceVariable { get; private set; }
        public string TargetVariable { get; private set; }
        public int Size { get; private set; }

        protected override void Initialize(Settings settings)
        {
            base.Initialize(settings);
            this.SourceVariable = settings.GetString("sourcevariable");
            this.TargetVariable = settings.GetString("targetvariable");
            this.Size = settings.GetInt("size");
        }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            base.LoadDefaultParameters(defaultParameter);
            var spl = defaultParameter.Split('>');
            Settings["sourcevariable"] = spl[0];
            Settings["targetvariable"] = spl[1];
        }

        protected override bool Process(IInteraction parameters)
        {
            var url = Fallback<object>.From(parameters, this.SourceVariable).ToString();
            var gen = new QRCoder.PayloadGenerator.Url(url);
            var qrcode = new QRCoder.Base64QRCode();
            var genqrcode = new QRCoder.QRCodeGenerator();
            var result = genqrcode.CreateQrCode(gen);
            qrcode.SetQRCodeData(result);
            var base64String = qrcode.GetGraphic(this.Size);
            return WithBranch.TryProcess(new SimpleInteraction(parameters, TargetVariable, base64String));
        }
    }
}
