using System;

namespace ApiClientLib.Helpers
{
    internal static class UrlValidation
    {
        internal static string CombineAndValidateUrl(string url, UrlType urlType, string clientBaseUrl)
        {
            return urlType == UrlType.Absolute ? ValidatedUrl(url, string.Empty) : ValidatedUrl(clientBaseUrl, url);
        }

        internal static string ValidatedUrl(string baseUrl, string relativeUrl)
        {
            if (relativeUrl == null)
                throw new ArgumentNullException(nameof(relativeUrl), "Relative URL cannot be null.");

            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null.");

            if (Uri.TryCreate(relativeUrl, UriKind.Absolute, out _))
                throw new InvalidUrlException(relativeUrl, baseUrl, relativeUrl,
                    $"Invalid relative URL: '{relativeUrl}'. It cannot be absolute.");

            if (Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) &&
                baseUri.IsWellFormedOriginalString() &&
                Uri.TryCreate(baseUri, relativeUrl, out var uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) &&
                uriResult.IsWellFormedOriginalString())
            {
                return uriResult.AbsoluteUri;
            }

            var mergedUrl = baseUrl + relativeUrl;
            throw new InvalidUrlException(mergedUrl, baseUrl, relativeUrl,
                $"Invalid URL: '{mergedUrl}'. Must be an absolute and well-formed HTTP/HTTPS URL.");
        }
    }
}