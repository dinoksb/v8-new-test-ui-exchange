using System;
using System.Collections.Generic;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;
using G2.UI.Elements.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace G2.Converter
{
    public class ElementDataConverter : JsonConverter
    {
        private const string _TYPE = "type";

        public static readonly IList<JsonConverter> Converters = new List<JsonConverter> { new ElementDataConverter() };
        
        public override bool CanConvert(Type objectType)
        {
            return typeof(ElementData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var typeValue = (jObject[_TYPE] ?? throw new InvalidOperationException()).Value<string>();

            if (!Enum.TryParse(typeValue, ignoreCase: true, out ElementType type)) throw new NotSupportedException($"Unknown ElementData type: {typeValue}");
            
            var result = type switch
            {
                ElementType.Element => new ElementData(),
                ElementType.Frame => new FrameData(),
                ElementType.Image => new ImageData(),
                ElementType.Label => new LabelData(),
                ElementType.Button => new ButtonData(),
                _ => throw new ArgumentOutOfRangeException()
            };
                
            serializer.Populate(jObject.CreateReader(), result);
            return result;

        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jToken = JToken.FromObject(value);
            jToken.WriteTo(writer);
        }
    }
}
