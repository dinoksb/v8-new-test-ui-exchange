using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class CoordinateTransformData
    {
        [JsonProperty("x", Required = Required.Default)]
        public TransformData x;

        [JsonProperty("y", Required = Required.Default)]
        public TransformData y;

        public void Initialize()
        {
            x ??= new TransformData();
            y ??= new TransformData();
        }
    }
}