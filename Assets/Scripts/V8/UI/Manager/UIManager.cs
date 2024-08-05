using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;

        // private readonly Dictionary<string, IElement> _ui = new();
        private readonly Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();

        private IElement _canvas;
        private IElement Canvas => _canvas ??= new Canvas(UIConfig.Canvas, transform);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        private void Clear()
        {
            // DestroyImmediate(Canvas.Self.gameObject);
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
            foreach (var (uiKey, uiData) in uis)
            {
                _ui[uiKey] = CreateElement(uiData, uiKey);
            }
        }

        private IElement CreateElement(ElementData data, string id)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(data, parent, id);
            element.Visible = data.visible;
            return element;
        }

        private IElement GetParentFromElement(string key)
        {
            IElement parent = GetElement(key);
            return parent ?? Canvas;
        }

        private IElement GetElement(string key)
        {
            if (_ui.TryGetValue(key, out var element))
            {
                return element;
            }
            Debug.Log($"The key '{key}' was not found in the UI elements.");
            //throw new KeyNotFoundException($"The key '{key}' was not found in the UI elements.");
            return null;
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