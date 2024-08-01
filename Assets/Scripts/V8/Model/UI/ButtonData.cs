using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ButtonData : LayoutData
    {
        [JsonProperty("events", Required = Required.Always)]
        public Dictionary<string, string> events;
        [JsonProperty("threshold", Required = Required.Default)]
        public float threshold;
    }
}