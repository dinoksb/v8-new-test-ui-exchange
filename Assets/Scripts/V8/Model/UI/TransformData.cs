using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class TransformData
    {
        [JsonProperty("scale", Required = Required.Default)]
        public float scale = 0;

        [JsonProperty("offset", Required = Required.Default)]
        public float offset = 0;
    }
}