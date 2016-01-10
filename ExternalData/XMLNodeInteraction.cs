using BorrehSoft.ApolloGeese.CoreTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ExternalData
{
    class XMLNodeInteraction : SimpleInteraction
    {
        public XmlNode Node { get; private set; }

        public XMLNodeInteraction(IInteraction parameters, XmlNode node)
            : base(parameters)
        {   
            this.Node = node;

            this["node_name"] = this.Node.LocalName;

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    this[attrib.Name] = attrib.Value;
                }
            }            
        }

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
