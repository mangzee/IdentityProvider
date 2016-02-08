using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microchip.SAML.IDPLibrary
{
    public class AuthnRequest
    {
        public string ID { get; set; }
        public string Version { get; set; }
        public string ProtocolBinding { get; set; }
        public string AssertionConsumerServiceURL { get; set; }
        public string Issuer { get; set; }
        public string IssueInstant { get; set; }
        public AuthnRequest(XmlElement xml)
        {
            XmlDocument xmlDoc = xml.OwnerDocument;
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            XmlNode Authnode = xmlDoc.SelectSingleNode("/samlp:AuthnRequest", manager);
            ID = Authnode.Attributes["ID"].Value;
            Version = Authnode.Attributes["Version"].Value;
            ProtocolBinding = Authnode.Attributes["ProtocolBinding"].Value;
            AssertionConsumerServiceURL = Authnode.Attributes["AssertionConsumerServiceURL"].Value;
            IssueInstant = Authnode.Attributes["IssueInstant"].Value;

            XmlNode IssuerNode = xmlDoc.SelectSingleNode("/samlp:AuthnRequest/saml:Issuer", manager);
            Issuer = IssuerNode.InnerText;

        }
    }
}
