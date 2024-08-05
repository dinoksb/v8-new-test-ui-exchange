using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace V8
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
        
        // public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        // {
        //     var jObject = JObject.Load(reader);
        //     var type = (string)jObject[Type];
        //     var instance = CreateInstance(type);
        //
        //     var childrenToken = jObject[Children];
        //     jObject.Remove(Children);
        //
        //     serializer.Populate(jObject.CreateReader(), instance);
        //
        //     if (childrenToken == null) return instance;
        //
        //     instance.children = new List<ElementData>();
        //     foreach (var childToken in childrenToken.Children<JObject>())
        //     {
        //         var childInstance = ReadJson(childToken.CreateReader(), objectType, null, serializer) as ElementData;
        //         instance.children.Add(childInstance);
        //     }
        //
        //     return instance;
        // }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jToken = JToken.FromObject(value);
            jToken.WriteTo(writer);
        }

        private static ElementData CreateInstance(string type)
        {
            return type switch
            {
                nameof(Layout) => new LayoutData(),
                nameof(Image) => new ImageData(),
                nameof(Label) => new LabelData(),
                nameof(Button) => new ButtonData(),
                _ => new ElementData()
            };
        }
    }
}