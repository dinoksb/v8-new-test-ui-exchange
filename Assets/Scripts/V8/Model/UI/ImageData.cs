using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ImageData : LayoutData
    {
        [JsonProperty("spriteId", Required = Required.Default)]
        public string spriteId;

        [JsonProperty("imageColor", Required = Required.Default)]
        public float[] imageColor = new float[] { 255, 255, 255, 255 };
    }
}