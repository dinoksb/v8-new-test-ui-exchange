using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public class ScrollFrameData : FrameData
    {
        [JsonProperty("scrollAxis", Required = Required.Always)]
        public string ScrollAxis { get; set; }
        
        [JsonProperty("contentAreaSize", Required = Required.Default)]
        public DimensionData ContentAreaSize { get; set; } = new();
        
        [JsonProperty("contentAreaAnchor", Required = Required.Default)]
        public float[] ContentAreaAnchor { get; set; } = { 0.5f, 0.5f };

        [JsonProperty("contentAreaPivot", Required = Required.Default)]
        public float[] ContentAreaPivot { get; set; } = { 0.5f, 0.5f };
        
        [JsonProperty("backgroundColor", Required = Required.Default)]
        public float[] BackgroundColor { get; set; } = { 1, 1, 1, 1 };
    }
}