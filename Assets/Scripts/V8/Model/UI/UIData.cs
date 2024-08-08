using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public struct UIData
    {
        [JsonProperty("studioData", Required = Required.AllowNull)]
        public StudioData studioData;
        [JsonProperty("asset", Required = Required.AllowNull)]
        public AssetData asset;
        [JsonProperty("ui", Required = Required.AllowNull)]
        public Dictionary<string, ElementData> ui;
    }
}