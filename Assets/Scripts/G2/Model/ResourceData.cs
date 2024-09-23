using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace G2.Model
{
    [Serializable]
    public class ResourceData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;
        [JsonProperty("url", Required = Required.Always)]
        public string Url;
    }
}
