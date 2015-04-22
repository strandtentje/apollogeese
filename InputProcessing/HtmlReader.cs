using BorrehSoft.ApolloGeese.Duckling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
    class HtmlReader : Service
    {
        public override string Description
        {
            get {
                return "parses html"; 
            }
        }

        List<NamedSelector> namedSelectors = new List<NamedSelector>();
        private Service Successful;
        private bool useVariable;
        private string variable;
        
        protected override void Initialize(Settings modSettings)
        {
            if (modSettings.Has("selection"))
            {
                useVariable = true;
                variable = modSettings.GetString("selection");
            }

            foreach (KeyValuePair<string, object> selectorPair in modSettings.Dictionary)
            {
                if (selectorPair.Key.EndsWith("_selector"))
                {
                    namedSelectors.Add(new NamedSelector(
                        selectorPair.Key.Remove(selectorPair.Key.Length - "_selector".Length),
                        (string)selectorPair.Value));
                }
            }
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name == "successful")
                Successful = e.NewValue;
        }

        /// <summary>
        /// Get existing C# HTML querier from context
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CQ GetCQFromContext(IInteraction parameters)
        {
            CQ querier;

            object possibleQuerier;
            if (parameters.TryGetFallback(variable, out possibleQuerier))
            {
                if (possibleQuerier is CQ)                
                    querier = (CQ)possibleQuerier;
                
                else                
                    querier = new CQ(possibleQuerier.ToString());
                
            }
            else            
                throw new ArgumentNullException(string.Format("nothing named {0} available in context", variable));            

            return querier;
        }

        /// <summary>
        /// Produce a new C# HTML querier using the incoming stream.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CQ GetCQFromStream(IInteraction parameters)
        {
            IIncomingBodiedInteraction incoming = (IIncomingBodiedInteraction)parameters.GetClosest(typeof(IIncomingBodiedInteraction));
            string document = incoming.GetIncomingBodyReader().ReadToEnd();
            return new CQ(document);
        }

        protected override bool Process(IInteraction parameters)
        {
            CQ querier = useVariable ? GetCQFromContext(parameters) : GetCQFromStream(parameters);

            QuickInteraction results = new QuickInteraction(parameters);

            CQ selection;
            string[] seperators = new string[] { "..." };
            string[] tags;
            string opener, inner, closer;

            foreach (NamedSelector namedSelector in namedSelectors)
            {
                selection = querier[namedSelector.selector];
                if (selection.Length > 0)
                {
                    tags = selection.ToString().Split(seperators, StringSplitOptions.RemoveEmptyEntries);

                    results[namedSelector.name + "_opener"] = opener = tags[0];
                    results[namedSelector.name + "_inner"] = inner = selection.Html();

                    if (tags.Length > 1) results[namedSelector.name + "_closer"] = closer = tags[1];

                    results[namedSelector.name] = selection;
                }
            }

            return Successful.TryProcess(results);
        }
    }

    struct NamedSelector
    {
        public string name;
        public string selector;

        public NamedSelector(string name, string selector)
        {
            this.name = name;
            this.selector = selector;
        }

    }
}
