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
    public partial class SLOService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RecieveSAMLSLORequest sloreq = new RecieveSAMLSLORequest();
            LogoutRequest logoutrequest = sloreq.RecieveSLO(Request);
            SendSAMLSLOResponse sloresp = new SendSAMLSLOResponse();
            FormsAuthentication.SignOut();
            sloresp.SendSLO(Response, User.Identity.Name, logoutrequest);
        }
    }
}