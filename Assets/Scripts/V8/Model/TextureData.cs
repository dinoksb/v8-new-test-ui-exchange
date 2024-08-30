using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class TextureData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string name;
        [JsonProperty("url", Required = Required.Always)]
        public string url;
    }
}