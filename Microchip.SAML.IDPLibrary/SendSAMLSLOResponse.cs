using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Microchip.SAML.IDPLibrary
{
    public class SendSAMLSLOResponse
    {
        public void SendSLO(HttpResponse httpResponse, string userName, LogoutRequest logoutRequest)
        {
            string samlResponse = CreateSAMLResponse(userName,logoutRequest);
            SendResponse(httpResponse, logoutRequest.SingleLogoutServiceUrl, "SAMLResponse", samlResponse);
        }

        private void SendResponse(HttpResponse httpResponse, string URL, string PostFormVariable, string samlResponse)
        {
            List<string> SPLogOutURLs = ConfigurationManager.AppSettings["SPLogOutURLs"].Split(',').Select(s => s.ToString()).ToList();
            if(SPLogOutURLs.Count>0)
            {
                Uri currentURI = new Uri(URL.ToLower());
                foreach (string s in SPLogOutURLs)
                {
                    Uri sURL = new Uri(s.ToLower());
                    var result = Uri.Compare(currentURI, sURL, UriComponents.Host | UriComponents.PathAndQuery, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
                    if (result != 0)
                    {
                        using (var client = new WebClient())
                        {
                            var values = new NameValueCollection();
                            values[PostFormVariable] = HttpUtility.HtmlEncode(samlResponse);
                            var response = client.UploadValues(s, values);
                            //var responseString = Encoding.Default.GetString(response);
                        }
                    }
                }
            }
            SAMLForm samlForm = new SAMLForm();
            samlForm.ActionURL = URL;
            samlForm.AddHiddenControl(PostFormVariable, samlResponse);
            samlForm.Write(httpResponse);
        }

        private string CreateSAMLResponse(string userName,LogoutRequest logoutRequest)
        {
            SLOSamlResponse SAMLResponse = new SLOSamlResponse(logoutRequest);
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;

                using (XmlWriter xw = XmlWriter.Create(sw, xws))
                {
                    xw.WriteStartElement("samlp", "LogoutResponse", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("ID", SAMLResponse.ID);
                    xw.WriteAttributeString("Version", SAMLResponse.Version);
                    xw.WriteAttributeString("IssueInstant", SAMLResponse.IssueInstant);
                    xw.WriteAttributeString("InResponseTo", SAMLResponse.InResponseTo);

                    xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString(SAMLResponse.Issuer);
                    xw.WriteEndElement();

                    xw.WriteStartElement("samlp", "Status", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteStartElement("samlp", "StatusCode", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("Value", SAMLResponse.status);
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteEndElement();
                }
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(sw.ToString());
                X509Certificate2 xcert = new X509Certificate2(HttpRuntime.AppDomainAppPath + "\\" + "idp.pfx", "password");
                AppendSignatureToXMLDocument(ref xmldoc, "", xcert);
                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(xmldoc.OuterXml.ToString());
                return System.Convert.ToBase64String(toEncodeAsBytes);
            }
            return null;

        }

        public void AppendSignatureToXMLDocument(ref XmlDocument XMLSerializedSAMLResponse, String ReferenceURI, X509Certificate2 SigningCert)
        {
            SignedXml signedXML = new SignedXml(XMLSerializedSAMLResponse);

            signedXML.SigningKey = SigningCert.PrivateKey;
            signedXML.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            Reference reference = new Reference();
            reference.Uri = ReferenceURI;//#Response
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXML.AddReference(reference);
            signedXML.ComputeSignature();

            XmlElement signature = signedXML.GetXml();

            XmlElement xeResponse = XMLSerializedSAMLResponse.DocumentElement;

            xeResponse.AppendChild(signature);
        }
    }
}
