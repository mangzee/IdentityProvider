using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Microchip.SAML.IDPLibrary
{
    public class SendSAMLSSOResponse
    {
        public void SendSSO(HttpResponse httpResponse, string userName, IDictionary<string, string> attributes, AuthnRequest authnRequest)
        {
            string samlResponse = CreateSAMLResponse(userName, attributes, authnRequest);
            SendResponse(httpResponse, authnRequest.AssertionConsumerServiceURL, "SAMLResponse", samlResponse);
        }

        private void SendResponse(HttpResponse httpResponse, string URL, string PostFormVariable, string samlResponse)
        {
            SAMLForm samlForm = new SAMLForm();
            samlForm.ActionURL = URL;
            samlForm.AddHiddenControl(PostFormVariable, samlResponse);
            samlForm.Write(httpResponse);
        }

        private string CreateSAMLResponse(string userName, IDictionary<string, string> attributes, AuthnRequest authnRequest)
        {
            SSOSamlResponse SAMLResponse = new SSOSamlResponse(authnRequest);
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;

                using (XmlWriter xw = XmlWriter.Create(sw, xws))
                {
                    xw.WriteStartElement("samlp", "Response", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("ID", SAMLResponse.ID);
                    xw.WriteAttributeString("Version", SAMLResponse.Version);
                    xw.WriteAttributeString("IssueInstant", SAMLResponse.IssueInstant);
                    xw.WriteAttributeString("Destination", SAMLResponse.Destination);
                    xw.WriteAttributeString("InResponseTo", SAMLResponse.InResponseTo);

                    xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString(SAMLResponse.Issuer);
                    xw.WriteEndElement();

                    //assertion
                    xw.WriteStartElement("saml", "Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteAttributeString("Version", SAMLResponse.Version);
                    xw.WriteAttributeString("ID", SAMLResponse.ID);
                    xw.WriteAttributeString("IssueInstant", SAMLResponse.IssueInstant);
                    xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString(SAMLResponse.Issuer);
                    xw.WriteEndElement();
                    xw.WriteStartElement("saml", "Subject", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteStartElement("saml", "NameID", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");
                    xw.WriteString(userName);
                    xw.WriteEndElement();
                    xw.WriteStartElement("saml", "SubjectConfirmation", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteAttributeString("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer");
                    xw.WriteStartElement("saml", "SubjectConfirmationData", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteAttributeString("Recipient", authnRequest.AssertionConsumerServiceURL);
                    xw.WriteAttributeString("InResponseTo", SAMLResponse.InResponseTo);
                    xw.WriteEndElement();//subjectconfiramationdata
                    xw.WriteEndElement();//subjectconfiramation
                    xw.WriteEndElement();//subject
                    xw.WriteStartElement("saml", "AuthnStatement", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteAttributeString("AuthnInstant", authnRequest.IssueInstant);
                    // xw.WriteAttributeString("SessionIndex", authnRequest.IssueInstant); //what to do?
                    xw.WriteStartElement("saml", "AuthnContext", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteStartElement("saml", "AuthnContextClassRef", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified");
                    xw.WriteEndElement();//AuthnContextClassRef
                    xw.WriteEndElement();//AuthnContext
                    xw.WriteEndElement();//AuthnStatement
                    xw.WriteStartElement("saml", "AttributeStatement", "urn:oasis:names:tc:SAML:2.0:assertion");
                    foreach (KeyValuePair<string, string> kvp in attributes)
                    {
                        xw.WriteStartElement("saml", "Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");
                        xw.WriteAttributeString("Name", kvp.Key);
                        xw.WriteStartElement("saml", "AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion");
                        //xw.WriteAttributeString("xsi:type", "xs:string");
                        xw.WriteString(kvp.Value);
                        xw.WriteEndElement();//AttributeValue
                        xw.WriteEndElement();//Attribute
                    }
                    xw.WriteEndElement();//AttributeStatement
                    xw.WriteEndElement();//assertion

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
