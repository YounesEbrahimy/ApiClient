using System;

namespace ApiClientLib
{
    public class InvalidUrlException : Exception
    {
        public string Url { get; }
        public string BaseUrl { get; }
        public string RelativeUrl { get; }

        public InvalidUrlException(string url, string baseUrl, string relativeUrl, string message) : base(message)
        {
            Url = url;
            BaseUrl = baseUrl;
            RelativeUrl = relativeUrl;
        }
    }
}