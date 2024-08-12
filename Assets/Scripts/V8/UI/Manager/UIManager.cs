using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;

        private Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        private IElement _canvas;

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
            Debug.Log($"[Load] : {json}");
            var uiData = UIJsonImporter.Import(json) ?? throw new ArgumentNullException("UIImporter.Import()");
            var studio = uiData.studioData;
            var asset = uiData.asset;
            var ui = uiData.ui;
            Vector2 referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            _sprites = await SpriteImporter.Import(asset.sprite, true);
            _canvas = new Canvas(UIConfig.Canvas, null, referenceResolution);
            BuildUI(ui, referenceResolution);
        }

        private void BuildUI(Dictionary<string, ElementData> uis, Vector2 referenceResolution)
        {
            foreach (var (key, element) in uis)
            {
                if (_ui.ContainsKey(key)) continue;

                _ui.Add(key, CreateElement(element, referenceResolution));
            }
        }

        private IElement CreateElement(ElementData data, Vector2 referenceResolution)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(data, parent);
            element.Visible = data.visible;
            return element;
        }

        private IElement GetParentFromElement(string id)
        {
            return GetElement(id) ?? _canvas;
        }

        private IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }
    }
}