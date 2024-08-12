using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class ButtonData : FrameData
    {
        public enum ButtonType
        {
            IMAGE_BUTTON,
            TEXT_BUTTON,
        }

        [JsonProperty("buttonType", Required = Required.Always)]
        public ButtonType buttonType = ButtonType.IMAGE_BUTTON;

        [JsonProperty("events", Required = Required.Always)]
        public Dictionary<string, string> events;

        [JsonProperty("threshold", Required = Required.Default)]
        public float threshold = 0.5f;

        [JsonProperty("defaultSpriteId", Required = Required.Always)]
        public string defaultSpriteId;

        [JsonProperty("hoverSpriteId", Required = Required.Default)]
        public string hoverSpriteId;

        [JsonProperty("pressedSpriteId", Required = Required.Default)]
        public string pressedSpriteId;
        
        // [JsonProperty("text", Required = Required.Default)]
        // public string text;
    }
}