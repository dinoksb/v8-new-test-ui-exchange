using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class FrameData : ElementData
    {
        [JsonProperty("interactable", Required = Required.Default)]
        public bool interactable = true;
    }
}