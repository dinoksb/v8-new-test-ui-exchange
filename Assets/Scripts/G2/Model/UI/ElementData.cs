using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace G2.Model.UI
{
    [Serializable]
    public class ElementData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;
        
        [JsonProperty("type", Required = Required.Always)]
        public string Type;
        
        [JsonProperty("parent", Required = Required.Default)]
        public string Parent = string.Empty;

        [JsonProperty("anchor", Required = Required.Default)]
        public float[] Anchor = new [] {0.5f, 0.5f};

        [FormerlySerializedAs("pivot")] [JsonProperty("pivot", Required = Required.Default)]
        public float[] Pivot = new [] {0.5f, 0.5f};

        [JsonProperty("position", Required = Required.Default)]
        public DimensionData Position = new();
        
        [JsonProperty("size", Required = Required.Default)]
        public DimensionData Size = new();

        [JsonProperty("rotation", Required = Required.Default)]
        public float Rotation = 0;

        [JsonProperty("visible", Required = Required.Default)]
        public bool Visible = true;
        
        [JsonProperty("zIndex", Required = Required.Default)]
        public uint ZIndex = 1;
    }
}
