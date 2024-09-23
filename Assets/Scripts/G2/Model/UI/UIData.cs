using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public struct UIData
    {
        [JsonProperty("studioData", Required = Required.Always)]
        public StudioData StudioData;

        [JsonProperty("textures", Required = Required.Always)]
        public Dictionary<string, ResourceData> Textures;

        [JsonProperty("spriteSheets", Required = Required.Always)]
        public Dictionary<string, SpriteSheetData> SpriteSheets;

        [JsonProperty("ui", Required = Required.Always)]
        public Dictionary<string, ElementData> UI;
    }
}
