using System;
using System.Collections.Generic;
using G2.Model.UI;
using G2.UI.Elements.Basic;
using G2.UI.Elements.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace G2.Converter
{
    public class ElementDataConverter : JsonConverter
    {
        private const string Type = "type";
        [Obsolete]
        private const string Children = "children";

        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            Converters = new List<JsonConverter> { new ElementDataConverter() }
        };
        
        public override bool CanConvert(Type objectType)
        {
            return typeof(ElementData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = (string)jObject[Type];
            var instance = CreateInstance(type);
            serializer.Populate(jObject.CreateReader(), instance);

            return instance;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jToken = JToken.FromObject(value);
            jToken.WriteTo(writer);
        }

        private static ElementData CreateInstance(string type)
        {
            return type switch
            {
                nameof(Frame) => new FrameData(),
                nameof(Image) => new ImageData(),
                nameof(Label) => new LabelData(),
                nameof(Button) => new ButtonData(),
                _ => new ElementData()
            };
        }
    }
}