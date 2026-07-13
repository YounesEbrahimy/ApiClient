using System.Collections.Generic;

namespace ApiClientLib.SubClasses
{
    internal sealed class HeaderManager : IHeaderManager
    {
        private readonly Dictionary<string, string> _persistentHeaders = new();

        public IReadOnlyDictionary<string, string> Headers => _persistentHeaders;
        public void AddHeader(string key, string value) => _persistentHeaders[key] = value;
        public void RemoveHeader(string key) => _persistentHeaders.Remove(key);
        public void ClearHeaders() => _persistentHeaders.Clear();
    }
}