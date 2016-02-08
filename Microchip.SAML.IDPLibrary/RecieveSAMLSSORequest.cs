using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Microchip.SAML.IDPLibrary
{
    public class RecieveSAMLSSORequest
    {
        public AuthnRequest RecieveSSO(HttpRequest request)
        {
            XmlElement authnRequestElement = (XmlElement)null;
            if (request.RequestType == "GET")
            {
                RecieveRequest(request, out authnRequestElement);
            }
            AuthnRequest authRequest = new AuthnRequest(authnRequestElement);
            return authRequest;
        }

        private void RecieveRequest(HttpRequest request, out XmlElement authnRequestElement)
        {
            ParseQueryString(request.RawUrl, "SAMLRequest", out authnRequestElement);
        }

        private void ParseQueryString(string redirectURL, string messageQueryName, out XmlElement authnRequestElement)
        {
            NameValueCollection nmc = ParseAndDecode(redirectURL);
            string encodedMSG = nmc[messageQueryName];
            authnRequestElement = DecodeMessage(encodedMSG);
        }

        private XmlElement DecodeMessage(string encodedMessage)
        {
            byte[] buffer1;
            try
            {
                buffer1 = Convert.FromBase64String(encodedMessage);
                string @string = Encoding.UTF8.GetString(buffer1);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;
                settings.XmlResolver = (XmlResolver)null;
                XmlDocument document = new XmlDocument();
                using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(@string), settings))
                    document.Load(reader);
                return document.DocumentElement;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private NameValueCollection ParseAndDecode(string url)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            int num = url.IndexOf('?');
            if (num >= 0 && num + 1 < url.Length)
            {
                string str1 = url.Substring(num + 1);
                char[] chArray = new char[1]
        {
          '&'
        };
                foreach (string str2 in str1.Split(chArray))
                {
                    if (str2.Length > 0)
                    {
                        int length = str2.IndexOf('=');
                        string name;
                        string queryValue;
                        if (length >= 0)
                        {
                            name = str2.Substring(0, length);
                            queryValue = str2.Substring(name.Length + 1);
                            queryValue = HttpUtility.UrlDecode(queryValue);
                        }
                        else
                        {
                            name = str2;
                            queryValue = string.Empty;
                        }
                        nameValueCollection.Add(name, queryValue);
                    }
                }
            }
            return nameValueCollection;
        }
    }
}
