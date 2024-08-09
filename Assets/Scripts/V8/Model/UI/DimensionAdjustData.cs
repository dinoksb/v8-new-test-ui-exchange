using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public struct DimensionAdjustData
    {
        [JsonProperty("scale", Required = Required.Default)]
        public float scale;

        [JsonProperty("offset", Required = Required.Default)]
        public float offset;

        public DimensionAdjustData(float scale, float offset)
        {
            this.scale = Math.Clamp(scale, 0, 1);
            this.offset = offset;
        }
    }
}
