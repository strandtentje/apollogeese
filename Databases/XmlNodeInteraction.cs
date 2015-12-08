using BorrehSoft.ApolloGeese.CoreTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Data
{
    class XMLNodeInteraction : SimpleInteraction
    {
        private IInteraction parameters;
        public XmlNode Node { get; private set; }

        public XMLNodeInteraction(IInteraction parameters, XmlNode node, IIncomingBodiedInteraction source)
            : base(parameters)
        {   
            this.Node = node;
            this.Source = source;

            this["node_name"] = this.Node.LocalName;

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    this[attrib.Name] = attrib.Value;
                }
            }            
        }

        public IIncomingBodiedInteraction Source { get; private set; }

        public override bool Has(string key)
        {
            return (key == "node_text") || base.Has(key);
        }

        public override object Get(string key)
        {
            if (key == "node_text")
            {
                return this.Node.InnerText;
            }
            else
            {
                return base.Get(key);
            }            
        }
    }
}
