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
    /// <summary>Spam protection applied to anonymous submissions.</summary>
    public enum CaptchaMode
    {
        None = 0,
        DnnCaptcha = 1,
        ReCaptchaV2 = 2,
        ReCaptchaV3 = 3,
        Turnstile = 4
    }

    /// <summary>
    ///   Server-side verification of Google reCAPTCHA (v2 checkbox, v3 score) and Cloudflare Turnstile
    ///   tokens. The secret goes in a POST body, the call has a timeout, and any failure - network,
    ///   parse, missing key - is treated as "not verified" (fail closed).
    /// </summary>
    public static class CaptchaVerifier
    {
        /// <summary>Action name the client passes to reCAPTCHA v3 / Turnstile and the server expects back.</summary>
        public const string ExpectedAction = "bccforms_submit";

        const string ReCaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        const string TurnstileVerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
        const int TimeoutMilliseconds = 10000;

        public static bool UsesProviderToken(CaptchaMode mode)
        {
            return mode == CaptchaMode.ReCaptchaV2 || mode == CaptchaMode.ReCaptchaV3 ||
                   mode == CaptchaMode.Turnstile;
        }

        /// <summary>Asks the provider whether <paramref name="token"/> is a valid, fresh, human token.</summary>
        public static bool Verify(CaptchaMode mode, string token, string secretKey, double minScore)
        {
            if (!UsesProviderToken(mode) || string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(secretKey))
            {
                return false;
            }

            VerifyResponse reply;
            try
            {
                reply = Post(mode == CaptchaMode.Turnstile ? TurnstileVerifyUrl : ReCaptchaVerifyUrl,
                             token, secretKey);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return false;
            }
            return Evaluate(mode, reply, minScore);
        }

        /// <summary>Decides from a provider reply; separate from the HTTP call so it can be tested.</summary>
        public static bool Evaluate(CaptchaMode mode, VerifyResponse reply, double minScore)
        {
            if (reply == null || !reply.Success) return false;

            switch (mode)
            {
                case CaptchaMode.ReCaptchaV2:
                    return true;
                case CaptchaMode.ReCaptchaV3:
                    // v3 always answers success for a well-formed token; the score and action decide.
                    return reply.Score.HasValue && reply.Score.Value >= minScore &&
                           string.Equals(reply.Action, ExpectedAction, StringComparison.Ordinal);
                case CaptchaMode.Turnstile:
                    // Turnstile echoes the action given to the widget; an empty action is tolerated.
                    return string.IsNullOrEmpty(reply.Action) ||
                           string.Equals(reply.Action, ExpectedAction, StringComparison.Ordinal);
                default:
                    return false;
            }
        }

        static VerifyResponse Post(string url, string token, string secretKey)
        {
            using (var client = new TimeoutWebClient(TimeoutMilliseconds))
            {
                var form = new NameValueCollection
                {
                    { "secret", secretKey },
                    { "response", token },
                };
                var remoteIp = HttpContext.Current?.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(remoteIp))
                {
                    form.Add("remoteip", remoteIp);
                }

                var json = Encoding.UTF8.GetString(client.UploadValues(url, "POST", form));
                return JsonConvert.DeserializeObject<VerifyResponse>(json);
            }
        }

        /// <summary>Common shape of the reCAPTCHA and Turnstile siteverify replies.</summary>
        public class VerifyResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("score")]
            public double? Score { get; set; }

            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("hostname")]
            public string Hostname { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

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
