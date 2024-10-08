using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public struct StudioData
    {
        [JsonProperty("version", Required = Required.Always)]
        public string version;  // studio version
        [JsonProperty("resolutionWidth", Required = Required.Always)]
        public uint resolutionWidth; // user resolution width
        [JsonProperty("resolutionHeight", Required = Required.Always)]
        public uint resolutionHeight; // user resolution height
    }
}
