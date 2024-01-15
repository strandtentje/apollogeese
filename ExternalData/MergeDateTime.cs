using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Globalization;

namespace ExternalData
{
    public class MergeDateTime : SingleBranchService
    {
        private string TargetVariable;
        private string FromDateVariable;
        private string FromTimeVariable;
        public override string Description => "Combines separate date and time back into iso datetime string";
        public override void LoadDefaultParameters(string defaultParameter)
        {
            var fromTo = defaultParameter.Split('>');
            if (fromTo.Length < 2) return;
            var toDateTime = fromTo[0].Split(',');
            Settings["targetvar"] = fromTo[1];
            Settings["fromdatevar"] = toDateTime[0];
            Settings["fromtimevar"] = toDateTime[1];
        }
        protected override void Initialize(Settings settings)
        {
            this.TargetVariable = settings.GetString("targetvar", "datetime");
            this.FromDateVariable = settings.GetString("fromdatevar", "datetime");
            this.FromTimeVariable = settings.GetString("fromtimevar", "datetime");
        }
        protected override bool Process(IInteraction parameters)
        {
            if (!parameters.TryGetFallbackString(FromDateVariable, out string sourceDate)) 
                throw new ArgumentException($"No date found under ${FromDateVariable}");
            if (!parameters.TryGetFallbackString(FromTimeVariable, out string sourceTime))
                throw new ArgumentException($"No time found under ${FromTimeVariable}");
            DateTime dateTime = DateTime.ParseExact($"{sourceDate} {sourceTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            return WithBranch.TryProcess(new SimpleInteraction(parameters, TargetVariable, dateTime.ToString("o")));
        }
    }
}
