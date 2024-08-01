using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class LabelData : LayoutData
    {
        [JsonProperty("textAlignment", Required = Required.Default)]
        public string textAlignment;
        [JsonProperty("fontId", Required = Required.Default)]
        public string fontId;
        [JsonProperty("fontColor", Required = Required.Default)]
        public float[] fontColor;
        [JsonProperty("fontSize", Required = Required.Default)]
        public float fontSize;
        [JsonProperty("minFontSize", Required = Required.Default)]
        public float minFontSize;
        [JsonProperty("characterSpacing", Required = Required.Default)]
        public float characterSpacing;
        [JsonProperty("lineSpacing", Required = Required.Default)]
        public float lineSpacing;
        [JsonProperty("autoSize", Required = Required.Default)]
        public bool autoSize;
        [JsonProperty("singleLine", Required = Required.Default)]
        public bool singleLine;
        [JsonProperty("ellipsis", Required = Required.Default)]
        public bool ellipsis;
        [JsonProperty("bold", Required = Required.Default)]
        public bool bold;
        [JsonProperty("italic", Required = Required.Default)]
        public bool italic;
        [JsonProperty("underline", Required = Required.Default)]
        public bool underline;
        [JsonProperty("strikethrough", Required = Required.Default)]
        public bool strikethrough;
        [JsonProperty("text", Required = Required.AllowNull)]
        public string text;
    }
}