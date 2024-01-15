using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Globalization;

namespace ExternalData
{
    public class SplitDateTime : SingleBranchService
    {
        private string SourceVariable;
        private string ToDateVariable;
        private string ToTimeVariable;

        public override string Description => "Parses or takes datetime and splits it into separate date and time";
        public override void LoadDefaultParameters(string defaultParameter)
        {
            var fromTo = defaultParameter.Split('>');
            if (fromTo.Length < 2) return;
            var toDateTime = fromTo[1].Split(',');
            Settings["sourcevar"] = fromTo[0];
            Settings["todatevar"] = toDateTime[0];
            Settings["totimevar"] = toDateTime[1];
        }

        protected override void Initialize(Settings settings)
        {
            this.SourceVariable = settings.GetString("sourcevar", "datetime");
            this.ToDateVariable = settings.GetString("todatevar", "datetime");
            this.ToTimeVariable = settings.GetString("totimevar", "datetime");
        }

        protected override bool Process(IInteraction parameters)
        {
            if (!parameters.TryGetFallback(SourceVariable, out object sourceData)) throw new ArgumentException($"No datetime found under ${SourceVariable}");
            DateTime dateTime;
            if (sourceData is DateTime alreadyDateTime) dateTime = alreadyDateTime;
            else if (sourceData is string stillAString) dateTime = DateTime.Parse(stillAString, null, DateTimeStyles.RoundtripKind);
            else throw new ArgumentException($"No datetime found under ${SourceVariable}");
            var translations = new Map<object>();
            translations[ToDateVariable] = dateTime.ToString("yyyy-MM-dd");
            translations[ToTimeVariable] = dateTime.ToString("HH:mm");
            return WithBranch.TryProcess(new SimpleInteraction(parameters, translations));
        }
    }
}
