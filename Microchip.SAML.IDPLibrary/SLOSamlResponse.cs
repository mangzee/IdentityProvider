using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microchip.SAML.IDPLibrary
{
    public class SLOSamlResponse
    {
        public string ID { get; set; }
        public string InResponseTo { get; set; }
        public string Version { get; set; }
        public string IssueInstant { get; set; }
        public string Issuer { get; set; }
        public string status { get; set; }

        public SLOSamlResponse(LogoutRequest logoutReq)
        {
            ID = "_" + System.Guid.NewGuid().ToString();
            InResponseTo = logoutReq.ID;
            Version = logoutReq.Version;
            IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            Issuer = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["IDPIssuer"])) ? "Microchip" : ConfigurationManager.AppSettings["IDPIssuer"];
            status = "urn:oasis:names:tc:SAML:2.0:status:Success";
        }
    }
}
