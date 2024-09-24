using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace G2.Model.UI
{
    [Serializable]
    public class DimensionData
    {
        [JsonProperty("x", Required = Required.Default)]
        public DimensionAdjustData X = new(0, 0);

        [JsonProperty("y", Required = Required.Default)]
        public DimensionAdjustData Y = new(0, 0);
    }
}
