using System;
using System.IO;
using System.Net;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using Ganss.Xss;

namespace BCC.Modules.BCCForms.Components
{
    /// <summary>
    ///   Cleans user-entered HTML before it is stored. Used when "input filtering" applies
    ///   (always for anonymous users; for everyone when the module forces it).
    /// </summary>
    /// <remarks>
    ///   Uses HtmlSanitizer (Ganss.Xss), which DNN 10 itself ships and uses, with an allowlist:
    ///   formatting markup, tables, lists, links and images are kept; script, event handlers,
    ///   javascript: URLs, embedded frames/objects and form controls are removed.
    ///   This replaces DNN's deprecated PortalSecurity.FilterFlag.NoScripting blocklist.
    ///
    ///   DNN 10.3.x ships AngleSharp (HtmlSanitizer's parser) without System.Memory / System.Buffers
    ///   (DNN 10.4 added them). On a server where those assemblies are not otherwise available the
    ///   sanitizer cannot load; BCC Forms then logs the error once and falls back to the legacy
    ///   NoScripting filter instead of failing the form.
    /// </remarks>
    internal static class HtmlFilter
    {
        static readonly object Sync = new object();
        static HtmlSanitizer _sanitizer;
        static bool _sanitizerUnavailable;

        /// <summary>Sanitizes HTML (rich-text fields). Returns HTML.</summary>
        public static string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html ?? string.Empty;
            }

            var sanitizer = GetSanitizer();
            return sanitizer != null ? sanitizer.Sanitize(html) : LegacyNoScripting(html);
        }

        /// <summary>
        ///   Sanitizes a plain-text field's input. Plain text comes back as typed (e.g. "Smith &amp; Sons"
        ///   stays "Smith & Sons"); input that still contains angle brackets after cleaning is kept in its
        ///   encoded HTML form, so decoding can never re-create markup.
        /// </summary>
        public static string SanitizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text ?? string.Empty;
            }

            var sanitizer = GetSanitizer();
            if (sanitizer == null)
            {
                return LegacyNoScripting(text);
            }

            var html = sanitizer.Sanitize(text);
            var decoded = WebUtility.HtmlDecode(html);
            return decoded.IndexOf('<') < 0 && decoded.IndexOf('>') < 0 ? decoded : html;
        }

        /// <summary>
        ///   Makes a stored value safe to show as HTML. Values without '&lt;' cannot contain markup and are
        ///   returned unchanged (cheap for the common case); anything else goes through the sanitizer, so
        ///   formatting survives but scripts, handlers and javascript: links do not.
        /// </summary>
        public static string SanitizeForDisplay(string value)
        {
            if (string.IsNullOrEmpty(value) || value.IndexOf('<') < 0)
            {
                return value ?? string.Empty;
            }
            return SanitizeHtml(value);
        }

        static readonly string[] SafeSchemes = { "http", "https", "mailto", "tel", "ftp", "ftps" };

        /// <summary>
        ///   True when <paramref name="url"/> is relative or uses a harmless scheme. Rejects
        ///   javascript:, vbscript:, data: and friends, including the forms browsers still accept
        ///   with tabs, newlines or control characters inside the scheme.
        /// </summary>
        public static bool IsSafeLinkUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            var scheme = new System.Text.StringBuilder();
            foreach (var c in url.Trim())
            {
                if (c == ':')
                {
                    var name = scheme.ToString().ToLowerInvariant();
                    return Array.IndexOf(SafeSchemes, name) >= 0;
                }
                if (c == '/' || c == '?' || c == '#')
                {
                    return true; // no scheme: relative URL
                }
                if (char.IsWhiteSpace(c) || char.IsControl(c))
                {
                    continue; // browsers strip these from a scheme
                }
                scheme.Append(c);
            }
            return true; // no ':' at all: relative URL (e.g. "page.aspx", "fileid=12", "57")
        }

        static HtmlSanitizer GetSanitizer()
        {
            if (_sanitizer != null || _sanitizerUnavailable)
            {
                return _sanitizer;
            }

            lock (Sync)
            {
                if (_sanitizer != null || _sanitizerUnavailable)
                {
                    return _sanitizer;
                }

                try
                {
                    _sanitizer = CreateSanitizer();
                }
                catch (Exception ex) when (IsLoadFailure(ex))
                {
                    _sanitizerUnavailable = true;
                    try
                    {
                        Exceptions.LogException(new InvalidOperationException(
                            "BCC Forms: HtmlSanitizer could not be loaded (on DNN 10.3.x this usually means " +
                            "System.Memory / System.Buffers are missing from bin). Falling back to the legacy " +
                            "script filter. Upgrading DNN to 10.4 or later resolves this.", ex));
                    }
                    catch (Exception)
                    {
                        // logging must never stop the fallback from working
                    }
                }
                return _sanitizer;
            }
        }

        static HtmlSanitizer CreateSanitizer()
        {
            // Configured once and then only used for Sanitize(), which HtmlSanitizer supports concurrently.
            var sanitizer = new HtmlSanitizer();

            // No form controls or document-level elements in stored content (phishing / page takeover).
            foreach (var tag in new[] { "form", "input", "button", "select", "option", "optgroup", "textarea",
                                        "datalist", "keygen", "output", "meter", "progress", "menu", "menuitem",
                                        "html", "head", "body" })
            {
                sanitizer.AllowedTags.Remove(tag);
            }

            // Editors use classes for formatting; links to email and phone numbers are common in forms.
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedSchemes.Add("mailto");
            sanitizer.AllowedSchemes.Add("tel");

            // Inline styles may format text but not lay content over the page.
            sanitizer.AllowedCssProperties.Remove("position");
            sanitizer.AllowedCssProperties.Remove("z-index");

            // Force the parser to load now, so a missing dependency is detected here, not mid-request.
            sanitizer.Sanitize("<p>ok</p>");
            return sanitizer;
        }

        static bool IsLoadFailure(Exception ex)
        {
            for (var e = ex; e != null; e = e.InnerException)
            {
                if (e is FileNotFoundException || e is FileLoadException || e is TypeLoadException ||
                    e is TypeInitializationException || e is MissingMethodException || e is BadImageFormatException)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>The pre-DNN 10 behaviour, kept only as the fallback described above.</summary>
        static string LegacyNoScripting(string input)
        {
            // PortalSecurity decodes input that contains "&lt;"/"&gt;" before filtering; prefixing an
            // encoded helper string forces that, and it is stripped again afterwards.
            input = WebUtility.HtmlEncode(string.Concat("<>", input));
#pragma warning disable 618 // CS0618: NoScripting is deprecated; used only when HtmlSanitizer is unavailable
            input = PortalSecurity.Instance.InputFilter(input, PortalSecurity.FilterFlag.NoScripting);
#pragma warning restore 618
            return WebUtility.HtmlDecode(input).Substring(2);
        }
    }
}
