using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microchip.SAML.IDPLibrary;
using System.Web.Security;

namespace IdentityProvider
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
         {
            if(User.Identity.IsAuthenticated)
            {
                divLogin.Style.Add("display","none");
            }
            else
                divLoggedIn.Style.Add("display", "none");

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // For simplicity, this example uses forms authentication with credentials stored in web.config.
            // Your application can use any authentication method you choose (eg Active Directory, custom database etc).
            // There are no restrictions on the method of authentication.
            if (FormsAuthentication.Authenticate(txtUname.Text,txtPwd.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(txtUname.Text, false);
            }
            else
            {
                lblError.Text = "Invalid credentials. The user name and password should be \"idp-user\" and \"password\".";
            }
        }
    }
}