using Microchip.SAML.IDPLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IdentityProvider
{
    public partial class SSOService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RecieveSAMLSSORequest ssoreq = new RecieveSAMLSSORequest();
            AuthnRequest authrequest = ssoreq.RecieveSSO(Request);
            if(!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            SendSAMLSSOResponse ssoresp = new SendSAMLSSOResponse();
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("Attribute1", "Preferred");
            attributes.Add("Attribute2", "India");
            ssoresp.SendSSO(Response,User.Identity.Name, attributes, authrequest);
        }
    }
}