using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ImageData : FrameData
    {
        [JsonProperty("spriteId", Required = Required.Always)]
        public string spriteId;
        
        [JsonProperty("backgroundColor", Required = Required.Default)]
        public float[] backgroundColor = new float[] { 1, 1, 1, 1 };
        
        [JsonProperty("imageColor", Required = Required.Default)]
        public float[] imageColor = new float[] { 1, 1, 1, 1 };
    }
}