using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;

        private readonly Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();

        private IElement _canvas;
        private IElement Canvas => _canvas ??= new Canvas(UIConfig.Canvas, transform);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        private void Clear()
        {
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
            foreach (var (key, element) in uis)
            {
                if (_ui.ContainsKey(key)) continue;

                _ui.Add(key, CreateElement(element));
            }
        }

        private IElement CreateElement(ElementData data)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(data, parent);
            element.Visible = data.visible;
            Debug.Log($"name: {element.Name}");
            return element;
        }

        private IElement GetParentFromElement(string id)
        {
            return GetElement(id) ?? Canvas;
        }

        private IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }

        // private IElement CreateElement(ElementData data, IElement parent)
        // {
        //     _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
        //     var factory = _factoryProvider.GetFactory(data.type);
        //     var element = factory.Create(data, parent);
        //     var children = CreateChildrenElement(data.children, element);
        //     element.Children.AddRange(children);
        //     element.Visible = data.visible;
        //     return element;
        // }

        // private IEnumerable<IElement> CreateChildrenElement(IEnumerable<ElementData> childrenData, IElement parent)
        // {
        //     return childrenData.Select(child => CreateElement(child, parent)).ToList();
        // }

        // public override UniTask<bool> Load(string verseAddress, string url, bool forceResourceDownload = false,
        //     CancellationToken cancellationToken = default)
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void Apply(string uiId, ElementData uiData)
        // {
        //     throw new System.NotImplementedException();
        // }

        // public override ElementData Get(string fileName)
        // {
        //     return null;
        // }
    }
}