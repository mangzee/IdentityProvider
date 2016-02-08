using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microchip.SAML.IDPLibrary
{
    public class LogoutRequest
    {
        public string ID { get; set; }
        public string Version { get; set; }
        public string IssueInstant { get; set; }
        public string SingleLogoutServiceUrl { get; set; }
        public string SingleLogoutServiceBinding { get; set; }
        public string Issuer { get; set; }
        public string NameID { get; set; }
        public string SessionIndex { get; set; }

        public LogoutRequest(XmlElement xml)
        {
            XmlDocument xmlDoc = xml.OwnerDocument;
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            XmlNode Authnode = xmlDoc.SelectSingleNode("/samlp:LogoutRequest", manager);
            ID = Authnode.Attributes["ID"].Value;
            Version = Authnode.Attributes["Version"].Value;
            SingleLogoutServiceUrl = Authnode.Attributes["SingleLogoutServiceUrl"].Value;
            SingleLogoutServiceBinding = Authnode.Attributes["SingleLogoutServiceBinding"].Value;           
            IssueInstant = Authnode.Attributes["IssueInstant"].Value;

            XmlNode IssuerNode = xmlDoc.SelectSingleNode("/samlp:LogoutRequest/saml:Issuer", manager);
            Issuer = IssuerNode.InnerText;

            XmlNode NameIDNode = xmlDoc.SelectSingleNode("/samlp:LogoutRequest/saml:NameID", manager);
            NameID = NameIDNode.InnerText;

            //XmlNode SessionIndexNode = xmlDoc.SelectSingleNode("/samlp:LogoutRequest/saml:SessionIndex", manager);
            //SessionIndex = SessionIndexNode.InnerText;
        }
    }
}
