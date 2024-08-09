using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public struct StudioData
    {
        [JsonProperty("version", Required = Required.Always)]
        public string version;  // studio version
        [JsonProperty("resolutionWidth", Required = Required.Always)]
        public int resolutionWidth; // user resolution width
        [JsonProperty("resolutionHeight", Required = Required.Always)]
        public int resolutionHeight; // user resolution height
    }
}