using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microchip.SAML.IDPLibrary
{
    public class SSOSamlResponse
    {
        public string ID { get; set; }
        public string InResponseTo { get; set; }
        public string Version { get; set; }
        public string IssueInstant { get; set; }
        public string Destination { get; set; }
        public string Issuer { get; set; }
        public string status { get; set; }

        public SSOSamlResponse(AuthnRequest authReq)
        {
            ID = "_" + System.Guid.NewGuid().ToString();
            InResponseTo = authReq.ID;
            Version = authReq.Version;
            IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            Destination = authReq.AssertionConsumerServiceURL;
            Issuer = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["IDPIssuer"])) ? "Microchip" : ConfigurationManager.AppSettings["IDPIssuer"];
            status = "urn:oasis:names:tc:SAML:2.0:status:Success";
        }
    }
}
