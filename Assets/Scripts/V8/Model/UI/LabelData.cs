using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class LabelData : LayoutData
    {
        [JsonProperty("textAlignment", Required = Required.Default)]
        public string textAlignment = "Left";

        [JsonProperty("fontId", Required = Required.Default)]
        public string fontId = "defaultFont";

        [JsonProperty("fontColor", Required = Required.Default)]
        public float[] fontColor = new float[] { 0, 0, 0, 0 };

        [JsonProperty("fontSize", Required = Required.Default)]
        public float fontSize = 36;

        [JsonProperty("minFontSize", Required = Required.Default)]
        public float minFontSize = 18;

        [JsonProperty("characterSpacing", Required = Required.Default)]
        public float characterSpacing = 0;

        [JsonProperty("lineSpacing", Required = Required.Default)]
        public float lineSpacing = 0;

        [JsonProperty("autoSize", Required = Required.Default)]
        public bool autoSize = false;

        [JsonProperty("singleLine", Required = Required.Default)]
        public bool singleLine = false;

        [JsonProperty("ellipsis", Required = Required.Default)]
        public bool ellipsis = false;

        [JsonProperty("bold", Required = Required.Default)]
        public bool bold = false;

        [JsonProperty("italic", Required = Required.Default)]
        public bool italic = false;

        [JsonProperty("underline", Required = Required.Default)]
        public bool underline = false;

        [JsonProperty("strikethrough", Required = Required.Default)]
        public bool strikethrough = false;

        [JsonProperty("text", Required = Required.AllowNull)]
        public string text = string.Empty;
    }
}