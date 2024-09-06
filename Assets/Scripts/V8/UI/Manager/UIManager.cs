using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using V8.Utilities;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        private Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        private IElement _tempCanvas;

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        public async void Load(string url)
        {
            Release();
            var uiData = UIJsonImporter.Import(url) ?? throw new ArgumentNullException("UIImporter.Import()");
            var studio = uiData.studioData;
            var asset = uiData.asset;
            var ui = uiData.ui;
            var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            _sprites = await SpriteImporter.Import(asset.texture, asset.sprite, Application.persistentDataPath,true);
            _tempCanvas = new Canvas(UIConfig.Canvas, null, referenceResolution, true);
            BuildUI(ui, referenceResolution);
            InternalDebug.Log($"ui json loaded. : {url}");
        }

        public void Show(UnityEngine.Canvas target)
        {
            var elementParents = _ui.Values.Where(x => CheckIsCanvas(x.Parent));
            foreach (var element in elementParents)
            {
                element.Self.SetParent(target.transform);
            }
        }

        public IElement Get(string uid)
        {
            if (_ui.TryGetValue(uid, out var element))
            {
                return element;
            }
            
            InternalDebug.Log($"[{uid}]: element does not exist.");
            return null;
        }

        public T Get<T>(string uid) where T : IElement
        {
            return (T)Get(uid);
        }

        public IEnumerable<IElement> GetFromName(string name)
        {
            var elements =_ui.Values.Where(element => element.Name.Equals(name));
            if (!elements.Any())
            {
                InternalDebug.Log($"[{name}]: element does not exist.");
            }
            return elements;
        }

        public void Delete(string uid)
        {
            if(_ui == null) return;
            if (_ui.Count == 0) return;
            
            _ui.Remove(uid);
            InternalDebug.Log($"[{uid}]: element deleted.");
        }
        
        public void Release()
        {
            if (_tempCanvas == null && (_sprites.Count == 0 || _ui.Count == 0)) return;
            
            DestroyImmediate(_tempCanvas.Self?.gameObject);
            foreach (var (_, sprite) in _sprites)
            {
                DestroyImmediate(sprite);
            }
            _sprites.Clear();
            _ui.Clear();
            _tempCanvas = null;
            InternalDebug.Log("[Clear]");
        }

        private void BuildUI(Dictionary<string, ElementData> uis, Vector2 referenceResolution)
        {
            foreach (var (key, element) in uis)
            {
                if (_ui.ContainsKey(key)) continue;

                _ui.Add(key, CreateElement(key, element, referenceResolution));
            }
        }

        private IElement CreateElement(string uid, ElementData data, Vector2 referenceResolution)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(uid, data, parent);
            element.Visible = data.visible;
            return element;
        }

        private IElement GetParentFromElement(string id)
        {
            return string.IsNullOrEmpty(id) ? _tempCanvas : GetElement(id);
        }

        private IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }

        private bool CheckIsCanvas(IElement element)
        {
            return element.Self.GetComponent<UnityEngine.Canvas>();
        }
    }
}