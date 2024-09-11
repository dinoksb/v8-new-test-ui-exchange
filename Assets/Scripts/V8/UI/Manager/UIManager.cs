using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using V8.Utilities;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        private Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        private IElement _dontDestoryCanvas;

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        public async UniTask LoadAsync(string url)
        {
            Release();
            try
            {
                var uiData = UIJsonImporter.Import(url);
                if (uiData == null)
                {
                    InternalDebug.LogError("Error: ui json load failed");
                    return;
                }

                var studio = uiData.Value.studioData;
                var asset = uiData.Value.asset;
                var ui = uiData.Value.ui;
                var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);

                _sprites = await SpriteImporter.Import(asset.texture, asset.sprite, Application.persistentDataPath, true);
                _dontDestoryCanvas = new Canvas(UIConfig.Canvas, null, referenceResolution, true);
                BuildUI(ui, referenceResolution);
            }
            catch (Exception e)
            {
                InternalDebug.LogException(e);
            }

            InternalDebug.Log($"ui json loaded. : {url}");
        }

        public void Show(UnityEngine.Canvas target)
        {
            var elementParents = _ui.Values.Where(IsRootElement);
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
            var elements = _ui.Values.Where(element => element.Name.Equals(name));
            if (!elements.Any())
            {
                InternalDebug.Log($"[{name}]: element does not exist.");
            }

            return elements;
        }

        public void Delete(string uid)
        {
            if (_ui == null) return;
            if (_ui.Count == 0) return;

            _ui.Remove(uid);
            InternalDebug.Log($"[{uid}]: element deleted.");
        }

        public void Release()
        {
            if (_dontDestoryCanvas == null && (_sprites.Count == 0 || _ui.Count == 0)) return;

            foreach (var (_, sprite) in _sprites)
            {
                Destroy(sprite);
            }

            foreach (var (_, ui) in _ui)
            {
                Destroy(ui.Self?.gameObject);
            }

            _sprites.Clear();
            _ui.Clear();
            Destroy(_dontDestoryCanvas.Self?.gameObject);
            _dontDestoryCanvas = null;
            Resources.UnloadUnusedAssets();
            InternalDebug.Log("ui element released");
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
            var frameData = (FrameData)data;
            var dimOpacity = frameData.dim;

            _factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, dimOpacity, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(uid, data, parent);
            element.Visible = data.visible;
            return element;
        }

        private IElement GetParentFromElement(string id)
        {
            return string.IsNullOrEmpty(id) ? _dontDestoryCanvas : GetElement(id);
        }

        private IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }

        private bool IsRootElement(IElement element)
        {
            var parentTransform = element.Parent.Self;
            var rootTransform = element.Self.root;
            return parentTransform == rootTransform;
        }
    }
}