using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public struct DimensionAdjustData
    {
        [JsonProperty("scale", Required = Required.Default)]
        public readonly float scale;

        [JsonProperty("offset", Required = Required.Default)]
        public readonly float offset;

        public DimensionAdjustData(float scale, float offset)
        {
            this.scale = Math.Clamp(scale, 0, 1);
            this.offset = offset;
        }
    }
}
