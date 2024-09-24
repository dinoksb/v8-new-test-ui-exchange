using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace G2.Model.UI
{
    [Serializable]
    public struct DimensionAdjustData
    {
        [JsonProperty("scale", Required = Required.Default)]
        public float Scale;

        [JsonProperty("offset", Required = Required.Default)]
        public float Offset;

        public DimensionAdjustData(float scale, float offset)
        {
            Scale = Math.Clamp(scale, 0, 1);
            Offset = offset;
        }
    }
}
