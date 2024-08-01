using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class SpriteData
    {
        [Obsolete]
        public string id;
        
        [JsonProperty("url", Required = Required.Always)]
        public string url;
        [JsonProperty("size", Required = Required.Always)]
        public int[] size;
        
        [JsonProperty("pixelsPerUnit", Required = Required.Default)]
        public float pixelsPerUnit;
        [JsonProperty("offset", Required = Required.Default)]
        public int[] offset;
        [JsonProperty("border", Required = Required.Default)]
        public int[] border; // (0=left, 1=bottom, 2=right, 3=top)
        [JsonProperty("pivot", Required = Required.Default)]
        public float[] pivot;
    }
}