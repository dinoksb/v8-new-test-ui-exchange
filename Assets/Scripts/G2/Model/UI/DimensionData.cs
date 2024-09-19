using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public class DimensionData
    {
        [JsonProperty("x", Required = Required.Default)]
        public DimensionAdjustData x = new(0, 0);

        [JsonProperty("y", Required = Required.Default)]
        public DimensionAdjustData y = new(0, 0);
    }
}
