using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ButtonData : FrameData
    {
        [JsonProperty("events", Required = Required.Always)]
        public Dictionary<string, string> events;
        [JsonProperty("threshold", Required = Required.Default)]
        public float threshold = 0.5f;
        // public string type = "none";    // image, text, none
        // public string normalSpriteId;
    }
}