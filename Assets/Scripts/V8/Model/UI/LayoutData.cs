using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class LayoutData : ElementData
    {
        [JsonProperty("backgroundColor", Required = Required.Default)]
        public float[] backgroundColor = new float[]{ 0, 0, 0, 0 };
    }
}