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

        [JsonProperty("anchorMin", Required = Required.Default)]
        public List<float> anchorMin;

        [JsonProperty("anchorMax", Required = Required.Default)]
        public List<float> anchorMax;

        [JsonProperty("pivot", Required = Required.Default)]
        public List<float> pivot;

        [JsonProperty("position", Required = Required.Default)]
        public Vector2 position = new Vector2(0, 0);

        [JsonProperty("scale", Required = Required.Default)]
        public Vector2 scale = new Vector2(1, 1);

        [JsonProperty("rotation", Required = Required.Default)]
        public float rotation = 0;
        
        [JsonProperty("size", Required = Required.Default)]
        public Vector2 size = new Vector2(0, 0);

        [JsonProperty("visible", Required = Required.Default)]
        public bool visible = true;
    }
}