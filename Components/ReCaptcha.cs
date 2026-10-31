using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;

namespace BCC.Modules.BCCForms.Components
{
    public class ReCaptcha
    {
        const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        const int TimeoutMilliseconds = 10000;

        /// <summary>
        ///   Verifies a reCAPTCHA token with Google. The secret is sent in a POST body (never in the
        ///   query string), the call has a timeout, and any failure is treated as "not verified".
        /// </summary>
        public static bool Validate(string encodedResponse, string privateKey)
        {
            if (string.IsNullOrWhiteSpace(encodedResponse) || string.IsNullOrWhiteSpace(privateKey))
            {
                return false;
            }

            try
            {
                using (var client = new TimeoutWebClient(TimeoutMilliseconds))
                {
                    var form = new NameValueCollection
                    {
                        { "secret", privateKey },
                        { "response", encodedResponse },
                    };
                    var remoteIp = HttpContext.Current?.Request.UserHostAddress;
                    if (!string.IsNullOrEmpty(remoteIp))
                    {
                        form.Add("remoteip", remoteIp);
                    }

                    var reply = Encoding.UTF8.GetString(client.UploadValues(VerifyUrl, "POST", form));
                    var captchaResponse = JsonConvert.DeserializeObject<ReCaptcha>(reply);
                    return captchaResponse != null && captchaResponse.Success;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return false;
            }
        }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

        class TimeoutWebClient : WebClient
        {
            readonly int _timeout;

            public TimeoutWebClient(int timeout)
            {
                _timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request != null)
                {
                    request.Timeout = _timeout;
                }
                return request;
            }
        }
    }
}
