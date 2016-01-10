using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace ExternalData
{
	class XML : ExternalDataService
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
			base.Initialize (settings);
            this.Path = settings.GetString("path", "");
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
			base.HandleBranchChanged (sender, e);
            if (e.Name == "anynode")            
                this.AnyNode = e.NewValue;
            if (e.Name == "nonode")
                this.NoNode = e.NewValue;            
        }

        protected override bool Process(IInteraction parameters)
        {
            XMLNodeInteraction preceeding = FindNode(parameters);
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
                    successful &= Branches[name].TryProcess(new XMLNodeInteraction(parameters, child));
                else if (this.AnyNode != null)
                    successful &= this.AnyNode.TryProcess(new XMLNodeInteraction(parameters, child));                
            }

            if ((children.Count == 0) && (this.NoNode != null))            
                successful &= this.NoNode.TryProcess(parameters);
            
            return successful;
        }

        private XMLNodeInteraction FindNode(IInteraction parameters)
        {
			TextReader reader;
			IInteraction candidate;
			XMLNodeInteraction nodeInteraction = null;
            
            // find an XmlNodeInteraction and hold on to that
            if (parameters.TryGetClosest(typeof(XMLNodeInteraction), out candidate))
                nodeInteraction = (XMLNodeInteraction)candidate;            
            
            // But if a new incoming body was acquired after that
			if (TryGetDatareader(parameters, nodeInteraction, out reader)) 
            {
                XmlDocument document = new XmlDocument();

				document.Load(reader);

				reader.Close ();

                nodeInteraction = new XMLNodeInteraction(parameters, document);                
            }

            if (nodeInteraction == null)            
                // bitter failure
                throw new Exception("Couldn't find or produce XML document");                        

            return nodeInteraction;
        }

        public string DefaultBranch { get; set; }

        public string TextBranch { get; set; }

        public string TextVariable { get; set; }

        public string Path { get; set; }

        public Service AnyNode { get; set; }

        public Service NoNode { get; set; }
    }
}
