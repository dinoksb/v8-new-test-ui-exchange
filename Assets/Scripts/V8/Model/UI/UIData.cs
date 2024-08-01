using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public struct UIData
    {
        [JsonProperty("asset", Required = Required.AllowNull)]
        public AssetData asset;
        [JsonProperty("ui", Required = Required.AllowNull)]
        public Dictionary<string, ElementData> ui;
    }
}