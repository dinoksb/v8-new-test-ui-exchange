using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public class FrameData : UpdatableElementData
    {
        [JsonProperty("dim", Required = Required.Default)]
        public float dim = 0;
    }
}
