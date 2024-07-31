using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace V8
{
    public class UIImporter : MonoBehaviour
    {
        private Dictionary<string, Sprite> _sprites = new();
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        private IElement _canvas;
        private IElement Canvas => _canvas ??= new Canvas(GetType().Name);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        private void Clear()
        {
            DestroyImmediate(Canvas.Self.gameObject);
            _canvas = null;
            Debug.Log("[Clear]");
        }

        public async void Load(string json)
        {
            Clear();
            var data = JsonConvert.DeserializeObject<UIData>(json, ElementDataConverter.SerializerSettings);
            _sprites = await SpriteImporter.Import(data.asset.sprite, true);
            BuildUI(data.ui);
            Debug.Log($"[Load] : {json}");
        }

        private void BuildUI(Dictionary<string, ElementData> uis)
        {
            foreach (var (_, uiData) in uis)
            {
                CreateElement(uiData, Canvas);
            }
        }

        private IElement CreateElement(ElementData data, IElement parent)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var element = factory.Create(data, parent);
            var children = CreateChildrenElement(data.children, element);
            element.Children.AddRange(children);
            element.Visible = data.visible;
            return element;
        }

        private IEnumerable<IElement> CreateChildrenElement(IEnumerable<ElementData> childrenData, IElement parent)
        {
            return childrenData.Select(child => CreateElement(child, parent)).ToList();
        }
    }
}