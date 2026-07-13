using System.Collections.Generic;

namespace ApiClientLib.SubClasses
{
    internal interface IHeaderManager
    {
        IReadOnlyDictionary<string, string> Headers { get; }
        public void AddHeader(string key, string value);
        public void RemoveHeader(string key);
        public void ClearHeaders();
    }
}