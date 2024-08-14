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
        
        // Todo: parent 를 GUID 로 해야할지 이름으로 해야할지? 고유한 ID 를 쓰는것이 맞을듯
        [JsonProperty("parent", Required = Required.Default)]
        public string parent = string.Empty;

        [JsonProperty("anchorMin", Required = Required.Default)]
        public List<float> anchorMin;

        [JsonProperty("anchorMax", Required = Required.Default)]
        public List<float> anchorMax;

        [JsonProperty("pivot", Required = Required.Default)]
        public List<float> pivot;

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