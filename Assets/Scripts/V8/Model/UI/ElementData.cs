using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ElementData
    {
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

        [JsonProperty("id", Required = Required.Default)]
        public string id;
        
        [JsonProperty("children", Required = Required.Default)]
        public List<ElementData> children;
        
        // Todo: children 을 제어할 일이 있는 경우 여기서 접근을 하는게 맞는지 고민해봐야함.
        // public ElementData GetChildById(string elementId)
        // {
        //     return GetChild(child => child.id.Equals(elementId));
        // }
        //
        // public ElementData GetChildByType(string elementType)
        // {
        //     return GetChild(child => child.type.Equals(elementType));
        // }
        //
        // public List<ElementData> GetChildrenByType(string elementType)
        // {
        //     var matchedElements = new List<ElementData>();
        //     if (type.Equals(elementType))
        //     {
        //         matchedElements.Add(this);
        //     }
        //
        //     foreach (var child in children)
        //     {
        //         matchedElements.AddRange(child.GetChildrenByType(elementType));
        //     }
        //
        //     return matchedElements;
        // }
        //
        // private ElementData GetChild(Func<ElementData, bool> condition)
        // {
        //     if (condition(this)) return this;
        //
        //     foreach (var child in children)
        //     {
        //         var childData = child.GetChild(condition);
        //         if (childData != null) return childData;
        //     }
        //
        //     return default;
        // }
    }
}