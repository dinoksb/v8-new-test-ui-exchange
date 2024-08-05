using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class CoordinateTransformData
    {
        [JsonProperty("x", Required = Required.Default)]
        public TransformData x = new TransformData();

        [JsonProperty("y", Required = Required.Default)]
        public TransformData y = new TransformData();
    }
}