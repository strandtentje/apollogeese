using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
    public class QRCode : Service
    {
        public override string Description => "Generates a QR code";

        public string SourceVariable { get; private set; }
        public int Size { get; private set; }

        protected override void Initialize(Settings settings)
        {
            base.Initialize(settings);
            this.SourceVariable = settings.GetString("sourcevariable");
            this.Size = settings.GetInt("size");
        }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            base.LoadDefaultParameters(defaultParameter);
            Settings["sourcevariable"] = defaultParameter;
        }

        protected override bool Process(IInteraction parameters)
        {
            var url = Fallback<object>.From(parameters, this.SourceVariable).ToString();
            var gen = new QRCoder.PayloadGenerator.Url(url);
            var qrcode = new QRCoder.SvgQRCode();
            var genqrcode = new QRCoder.QRCodeGenerator();
            var result = genqrcode.CreateQrCode(gen);
            qrcode.SetQRCodeData(result);
            var svgstring = qrcode.GetGraphic(this.Size);
            var svgbytes = Encoding.UTF8.GetBytes(svgstring);
            Closest<IOutgoingBodiedInteraction>.From(parameters).OutgoingBody.Write(svgbytes, 0, svgbytes.Length);
            return true;
        }
    }
}
