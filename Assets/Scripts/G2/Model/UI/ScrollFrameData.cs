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
        
        [JsonProperty("startCorner", Required = Required.Default)]
        public string StartCorner { get; set; } = GridLayoutGroup.Corner.UpperLeft.ToString();

        [JsonProperty("startAxis", Required = Required.Default)]
        public string StartAxis { get; set; } = GridLayoutGroup.Axis.Horizontal.ToString();
        
        [JsonProperty("childSize", Required = Required.Default)]
        public float[] ChildSize { get; set; } = { 100, 100 };

        [JsonProperty("childPadding", Required = Required.Default)]
        public int[] ChildPadding { get; set; } = { 0, 0, 0, 0 };
        
        [JsonProperty("childSpacing", Required = Required.Default)]
        public float[] ChildSpacing { get; set; } = { 0, 0 };
        
        [JsonProperty("childAlignment", Required = Required.Default)]
        public string ChildAlignment { get; set; } = TextAnchor.UpperLeft.ToString();
        
        [JsonProperty("childConstraintCount", Required = Required.Default)]
        public int ChildConstraintCount { get; set; } = 1;
        
        [JsonProperty("frameBackgroundSpriteId", Required = Required.Always)]
        public string FrameBackgroundSpriteId { get; set; }
        
        [JsonProperty("frameBackgroundColor", Required = Required.Default)]
        public float[] FrameBackgroundColor { get; set; } = { 1, 1, 1, 1 };
    }
}
