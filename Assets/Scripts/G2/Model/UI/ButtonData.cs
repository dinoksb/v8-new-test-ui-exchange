using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public class ButtonData : UpdatableElementData
    {
        [JsonProperty("events", Required = Required.Always)]
        public Dictionary<string, string> Events;
    }
}
