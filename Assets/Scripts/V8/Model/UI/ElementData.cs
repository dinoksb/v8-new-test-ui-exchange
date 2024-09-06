using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ElementData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string name;
        
        [JsonProperty("type", Required = Required.Always)]
        public string type;
        
        [JsonProperty("parent", Required = Required.Default)]
        public string parent = string.Empty;

        [JsonProperty("anchor", Required = Required.Default)]
        public float[] anchor = new [] {0.5f, 0.5f};

        [JsonProperty("pivot", Required = Required.Default)]
        public float[] pivot = new [] {0.5f, 0.5f};

        [JsonProperty("position", Required = Required.Default)]
        public DimensionData position = new();
        
        [JsonProperty("size", Required = Required.Default)]
        public DimensionData size = new();

        [JsonProperty("rotation", Required = Required.Default)]
        public float rotation = 0;

        [JsonProperty("visible", Required = Required.Default)]
        public bool visible = true;
    }
}