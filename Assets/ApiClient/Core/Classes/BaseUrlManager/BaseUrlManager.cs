using System;

namespace ApiClientLib.SubClasses
{
    internal sealed class BaseUrlManager : IBaseUrlManager
    {
        private string _baseUrl = string.Empty;

        public string BaseUrl => _baseUrl;

        public void SetBaseUrl(string baseUrl)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            if (baseUrl.Length == 0)
            {
                _baseUrl = string.Empty;
            }
            else
            {
                _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            }
        }
    }
}