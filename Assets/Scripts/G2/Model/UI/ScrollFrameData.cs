using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace G2.Model.UI
{
    [Serializable]
    public class ScrollFrameData : FrameData
    {
        [JsonProperty("scrollAxis", Required = Required.Always)]
        public string ScrollAxis { get; set; }
        
        [JsonProperty("frameBackgroundSpriteId", Required = Required.Always)]
        public string FrameBackgroundSpriteId { get; set; }
        
        [JsonProperty("frameBackgroundColor", Required = Required.Default)]
        public float[] FrameBackgroundColor { get; set; } = new float[] { 1, 1, 1, 1 };

        [JsonProperty("scrollbarBackgroundSpriteId", Required = Required.Always)]
        public string ScrollbarBackgroundSpriteId { get; set; }
        
        [JsonProperty("scrollbarBackgroundColor", Required = Required.Default)]
        public float[] ScrollbarBackgroundColor { get; set; } = new float[] { 1, 1, 1, 1 };

        [JsonProperty("scrollbarHandleSpriteId", Required = Required.Always)]
        public string ScrollbarHandleSpriteId { get; set; }
        
        [JsonProperty("scrollbarHandleColor", Required = Required.Default)]
        public float[] ScrollbarHandleColor { get; set; } = new float[] { 1, 1, 1, 1 };

        [JsonProperty("frameMaskSpriteId", Required = Required.Always)]
        public string FrameMaskSpriteId { get; set; }

        [JsonProperty("childSize", Required = Required.Default)]
        public float[] ChildSize { get; set; } = { 100, 100 };

        [JsonProperty("childSpacing", Required = Required.Default)]
        public float[] ChildSpacing { get; set; } = { 0, 0 };

        [JsonProperty("startCorner", Required = Required.Default)]
        public string StartCorner { get; set; } = GridLayoutGroup.Corner.UpperLeft.ToString();

        [JsonProperty("startAxis", Required = Required.Default)]
        public string StartAxis { get; set; } = GridLayoutGroup.Axis.Horizontal.ToString();

        [JsonProperty("childAlignment", Required = Required.Default)]
        public string ChildAlignment { get; set; } = TextAnchor.UpperLeft.ToString();

        [JsonProperty("itemConstraintCount", Required = Required.Default)]
        public int ChildConstraintCount { get; set; } = 1;
    }
}
