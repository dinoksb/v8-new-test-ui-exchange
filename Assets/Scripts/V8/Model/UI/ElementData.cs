using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Serialization;

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
        public List<float> anchorMin = new List<float>() { 0.5f, 0.5f };

        [JsonProperty("anchorMax", Required = Required.Default)]
        public List<float> anchorMax = new List<float>() { 0.5f, 0.5f };

        [JsonProperty("pivot", Required = Required.Default)]
        public List<float> pivot = new List<float>() { 0.5f, 0.5f };

        [JsonProperty("position", Required = Required.Default)]
        public CoordinateTransformData position = new CoordinateTransformData();

        [JsonProperty("size", Required = Required.Default)]
        public CoordinateTransformData size = new CoordinateTransformData();

        [JsonProperty("visible", Required = Required.Default)]
        public bool visible;
        // [JsonProperty("size", Required = Required.Default)]
        // public List<string> size;
        //[JsonProperty("position", Required = Required.Default)]
        //public List<string> position;

        [Obsolete] public List<ElementData> children;

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