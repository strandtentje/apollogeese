using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Data
{
    class XML : Service
    {
        public override string Description
        {
            get { return "XML Iterator"; }
        }

        protected override void Initialize(Settings settings)
        {
            this.DefaultBranch = settings.GetString("defaultbranch", "default");
            this.TextBranch = settings.GetString("textbranch", "text");
            this.TextVariable = settings.GetString("textvariable", "text");
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            
        }



        protected override bool Process(IInteraction parameters)
        {
            XmlNodeInteraction preceeding = FindNode(parameters);
            string name;
            bool successful = true;

            foreach (XmlNode child in preceeding.Node.ChildNodes)
            {
                name = child.LocalName.TrimStart('#');

                if (Branches.Has(name))
                {
                    successful &= Branches[name].TryProcess(new XmlNodeInteraction(parameters, child));
                }
            }

            return true;
        }

        private XmlNodeInteraction FindNode(IInteraction parameters)
        {
            IInteraction candidate;

            if (parameters.TryGetClosest(typeof(XmlNodeInteraction), out candidate))
            {
                XmlNodeInteraction xmlNodeInteraction = (XmlNodeInteraction)candidate;

                return xmlNodeInteraction;
            }
            else if (parameters.TryGetClosest(typeof(IIncomingBodiedInteraction), out candidate))
            {
                IIncomingBodiedInteraction source = (IIncomingBodiedInteraction)candidate;

                XmlDocument document = new XmlDocument();

                document.Load(source.IncomingBody);

                return new XmlNodeInteraction(parameters, document);
            }
            else
            {
                throw new Exception("Couldn't find or produce XML document");
            }
        }

        public string DefaultBranch { get; set; }

        public string TextBranch { get; set; }

        public string TextVariable { get; set; }
    }
}
