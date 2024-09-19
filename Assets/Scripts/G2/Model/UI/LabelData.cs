using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public class LabelData : FrameData
    {
        [JsonProperty("textAlignment", Required = Required.Default)]
        public string textAlignment = "Left";

        [JsonProperty("fontId", Required = Required.Default)]
        public string fontId = "defaultFont";

        [JsonProperty("fontColor", Required = Required.Default)]
        public float[] fontColor = new float[] { 0, 0, 0, 1 };

        [JsonProperty("fontSize", Required = Required.Default)]
        public float fontSize = 36;
   
        [JsonProperty("text", Required = Required.AllowNull)]
        public string text = string.Empty;
    }
}
