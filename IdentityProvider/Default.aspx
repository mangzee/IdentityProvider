<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IdentityProvider.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblError" runat="server"></asp:Label>
            <div id="divLogin" runat="server">
                <fieldset style="width: 200px">
                    <legend>Login Form:</legend>
                    <table>
                        <tr>
                            <td>UserName:
                            </td>
                            <td>
                                <asp:TextBox ID="txtUname" runat="server"></asp:TextBox><br>
                            </td>
                        </tr>
                        <tr>
                            <td>Password:
                            </td>
                            <td>
                                <asp:TextBox ID="txtPwd" runat="server" TextMode="Password"></asp:TextBox><br>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />

                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
            <div id="divLoggedIn" runat="server">
                <h1> Logged In as : <%=  User.Identity.Name %> </h1>
            </div>
        </div>
    </form>
</body>
</html>
