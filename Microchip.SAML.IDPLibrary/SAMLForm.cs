using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Microchip.SAML.IDPLibrary
{
    internal class SAMLForm
    {
        private static string htmlFormTemplate = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><body onload=\"document.forms.samlform.submit()\"><noscript><p><strong>Note:</strong> Since your browser does not support Javascript, you must press the Continue button once to proceed.</p></noscript><form id=\"samlform\" action=\"{0}\" method=\"post\"><div>{1}</div><noscript><div><input type=\"submit\" value=\"Continue\"/></div></noscript></form></body></html>";
        private IDictionary<string, string> hiddenControls = (IDictionary<string, string>)new Dictionary<string, string>();
        private const string defaultHTMLFormTemplate = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><body onload=\"document.forms.samlform.submit()\"><noscript><p><strong>Note:</strong> Since your browser does not support Javascript, you must press the Continue button once to proceed.</p></noscript><form id=\"samlform\" action=\"{0}\" method=\"post\"><div>{1}</div><noscript><div><input type=\"submit\" value=\"Continue\"/></div></noscript></form></body></html>";
        private string actionURL;

        /// <summary>
        /// Gets or sets the HTML form template that's used to construct the form to post.
        ///             The template must include two insertion parameters.
        ///             {0} is the URL to which the form is to be posted.
        ///             {1} is the hidden form variables including the SAML message.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The HTML form template.
        /// 
        /// </value>
        public static string HTMLFormTemplate
        {
            get
            {
                return SAMLForm.htmlFormTemplate;
            }
            set
            {
                SAMLForm.htmlFormTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the action URL.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The action URL.
        /// 
        /// </value>
        public string ActionURL
        {
            get
            {
                return this.actionURL;
            }
            set
            {
                this.actionURL = value;
            }
        }

        /// <summary>
        /// Gets or sets the hidden controls.
        ///             The hidden controls consist of one or more name/value pairs
        ///             that will be inserted into the form as hidden form variables.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The hidden controls.
        /// 
        /// </value>
        public IDictionary<string, string> HiddenControls
        {
            get
            {
                return this.hiddenControls;
            }
            set
            {
                this.hiddenControls = value;
            }
        }

        /// <summary>
        /// Adds a hidden control to the form.
        ///             The control will be inserted into the form as a hidden form variable.
        /// 
        /// </summary>
        /// <param name="controlName"/><param name="controlValue"/>
        public void AddHiddenControl(string controlName, string controlValue)
        {
            this.hiddenControls.Add(controlName, controlValue);
        }

        /// <summary>
        /// Writes the form to the output stream.
        /// 
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Write(Stream stream)
        {
            try
            {
                string str = this.ToString();
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.Write(str);
                streamWriter.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Writes the form to the HTTP response output stream.
        /// 
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        public void Write(HttpResponseBase httpResponse)
        {
            this.Write(httpResponse.OutputStream);
        }

        /// <summary>
        /// Writes the form to the HTTP response output stream.
        /// 
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        public void Write(HttpResponse httpResponse)
        {
            this.Write(httpResponse.OutputStream);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            foreach (string index in (IEnumerable<string>)this.hiddenControls.Keys)
            {
                string formVariable = this.hiddenControls[index];
                stringBuilder1.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", (object)index, (object)HttpUtility.HtmlEncode(formVariable));
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.AppendFormat(SAMLForm.htmlFormTemplate, (object)this.actionURL, (object)stringBuilder1.ToString());
            return stringBuilder2.ToString();
        }
    }
}
