using Unity.Plastic.Newtonsoft.Json;
using System;

namespace ApiClientLib
{
    internal class CacheEntry
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("cached_at")] public DateTime CachedAt { get; set; }
    }
}