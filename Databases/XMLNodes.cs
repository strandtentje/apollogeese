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

        public override void LoadDefaultParameters(string defaultParameter)
        {
            this.Settings["path"] = defaultParameter;
        }

        protected override void Initialize(Settings settings)
        {
            this.Path = settings.GetString("path", "");
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name == "anynode")            
                this.AnyNode = e.NewValue;
            
        }

        protected override bool Process(IInteraction parameters)
        {
            XmlNodeInteraction preceeding = FindNode(parameters);
            string name;
            bool successful = true;

            XmlNodeList children;

            if (this.Path.Length > 0) 
                children = preceeding.Node.SelectNodes(this.Path);
            
            else
                children = preceeding.Node.ChildNodes;
            

            foreach (XmlNode child in children)
            {
                name = child.LocalName.TrimStart('#');

                if (Branches.Has(name))
                    successful &= Branches[name].TryProcess(new XmlNodeInteraction(parameters, child));
                else if (this.AnyNode != null)
                    successful &= this.AnyNode.TryProcess(new XmlNodeInteraction(parameters, child));                
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

        public string Path { get; set; }

        public Service AnyNode { get; set; }
    }
}
