using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Tipset.Helpers.Sanitization
{
    /// <summary>
    /// Lightweight whitelist-based HTML sanitizer. No external dependencies.
    /// </summary>
    public static class HtmlSanitizer
    {
        // Tags allowed with no attributes
        private static readonly HashSet<string> AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "p", "br", "b", "strong", "i", "em", "u",
            "ul", "ol", "li", "h2", "h3", "blockquote"
        };

        public static string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Process each tag: keep allowed ones (stripping attributes),
            // keep <a href="..."> with safe href, strip everything else.
            var result = Regex.Replace(html, @"<(/?)(\w+)([^>]*)>", m =>
            {
                var slash   = m.Groups[1].Value;   // "/" for closing tags
                var tag     = m.Groups[2].Value;
                var attrs   = m.Groups[3].Value;

                if (AllowedTags.Contains(tag))
                    return $"<{slash}{tag}>";

                if (tag.Equals("a", StringComparison.OrdinalIgnoreCase))
                {
                    if (slash == "/")
                        return "</a>";

                    // Extract href and validate it is http/https
                    var hrefMatch = Regex.Match(attrs, @"href\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
                    if (hrefMatch.Success)
                    {
                        var href = hrefMatch.Groups[1].Value;
                        if (Uri.TryCreate(href, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                        {
                            return $"<a href=\"{HttpUtility.HtmlAttributeEncode(uri.ToString())}\" rel=\"noopener noreferrer\" target=\"_blank\">";
                        }
                    }
                    // No valid href → strip the tag entirely
                    return string.Empty;
                }

                // Unknown tag → strip it (keep inner text)
                return string.Empty;
            });

            return result;
        }
    }
}