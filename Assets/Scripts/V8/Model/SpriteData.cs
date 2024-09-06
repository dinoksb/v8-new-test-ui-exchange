using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace V8
{
    [Serializable]
    public class SpriteData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string name;

        [JsonProperty("textureId", Required = Required.Always)]
        public string textureId;

        [JsonProperty("size", Required = Required.Always)]
        public float[] size;

        [JsonProperty("offset", Required = Required.Default)]
        public float[] offset = new[] { 0.0f, 0.0f };

        [JsonProperty("border", Required = Required.Default)]
        public float[] border = new[] { 0.0f, 0.0f, 0.0f, 0.0f }; // (0=left, 1=bottom, 2=right, 3=top)

        [JsonProperty("pivot", Required = Required.Default)]
        public float[] pivot = new[] { 0.5f, 0.5f };

        [JsonProperty("pixelsPerUnit", Required = Required.Default)]
        public float pixelsPerUnit = 100f;
    }
}