using System;
using System.Collections;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using SimpleJson.Transcoder;

namespace ExternalData
{
    public class JsonPick : ExternalDataService
    {
        public override string Description => "Pick certain values from a json into a var name";

        public Service WithBranch { get; private set; }

        List<Tuple<string, string>> paths = new List<Tuple<string, string>>();

        protected override void Initialize(Settings settings)
        {
            foreach (var item in settings.Dictionary)
                if (item.Key.EndsWith("_override", StringComparison.Ordinal) && (item.Value is string))
                {
                    var varname = item.Key.Remove(item.Key.Length - "_override".Length);
                    var path = item.Value;
                    paths.Add(new Tuple<string, string>(path as string, varname));
                }        
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name == "_with") WithBranch = e.NewValue;

            base.HandleBranchChanged(sender, e);
        }

        protected override bool Process(IInteraction parameters)
        {
            if (TryGetDatareader(parameters, null, out var rdr))
            {
                var jsonObject = JsonSerializer.DeserializeString(rdr.ReadToEnd()) as Hashtable;
                var resultInteraction = new SimpleInteraction(parameters);

                foreach (var item in paths)
                {
                    jsonObject.TryGetString(item.Item1, out string result);
                    resultInteraction[item.Item2] = result;
                }

                return WithBranch.TryProcess(resultInteraction);
            }
            else
            {
                throw new Exception("No data found for jsonpick to consume");
            }
        }
    }
}
