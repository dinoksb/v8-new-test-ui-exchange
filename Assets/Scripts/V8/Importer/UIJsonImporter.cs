using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using V8.Utilities;

namespace V8
{
    public static class UIJsonImporter
    {
        private static IElement _tempCanvas;
        private static Dictionary<string, IElement> _ui = new();
        private static Dictionary<string, Sprite> _sprites = new();
        private static Dictionary<uint, UnityEngine.Canvas> _zIndexContainer = new();

        private static void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        public static UIData? Import(string url)
        {
            if (IsValidation(url))
            {
                try
                {
                    return JsonConvert.DeserializeObject<UIData>(url, ElementDataConverter.SerializerSettings);
                }
                catch (JsonException e)
                {
                    InternalDebug.LogException(e);
                }
            }
            else
            {
                InternalDebug.LogError("[UIJsonImporter] json format is invalid.");
            }

            return null;
        }

        public static void Release()
        {
            foreach (var (_, sprite) in _sprites)
            {
                Object.DestroyImmediate(sprite);
            }

            foreach (var (_, element) in _ui)
            {
                if(element.Self)
                    Object.DestroyImmediate(element.Self.gameObject);
            }
            
            GameObject rootUIObject = _tempCanvas?.Self ? _tempCanvas.Self.gameObject : FindRootObject();
            if (rootUIObject)
            {
                Object.DestroyImmediate(rootUIObject);
                _tempCanvas = null;
            }
            
            _sprites.Clear();
            _ui.Clear();
            _zIndexContainer.Clear();
            InternalDebug.Log("[UIJsonImporter] ui element released");
        }

        public static void ImportAndBuild(string filePath, bool isDevMode)
        {
            Release();
            var json = File.ReadAllText(filePath);
            UIData? uiData = Import(json);
            if (uiData == null)
            {
                InternalDebug.LogError("Error: ui json load failed");
                return;
            };
            BuildUI(uiData, isDevMode);
        }
        
        private static async void BuildUI(UIData? data, bool isDevMode)
        {
            var studio = data.Value.studioData;
            var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            var asset = data.Value.asset;
            var ui = data.Value.ui;

            _tempCanvas = new Canvas(UIConfig.Canvas, null, referenceResolution, false);
            _sprites = await SpriteImporter.Import(asset.resource, asset.sprite, Application.persistentDataPath, true);

            foreach (var (key, element) in ui)
            {
                if (_ui.ContainsKey(key)) continue;
                var createdElement = CreateElement(key, element, referenceResolution, isDevMode);
                if(isDevMode)
                    createdElement.Self.AddComponent<DevElementInfo>().Attach(createdElement);
                _ui.Add(key, createdElement);
            }
            
            await UniTask.Yield();

            foreach (var canvas in _zIndexContainer.Values)
                canvas.overrideSorting = true;
        }

        private static bool IsValidation(string json)
        {
            // TODO: Json Schema 를 통한 유효성 검사(필요할지?)
            return true;
        }

        private static GameObject FindRootObject()
        {
            var canvases = Object.FindObjectsByType<UnityEngine.Canvas>(FindObjectsSortMode.None);
            var canvas = canvases.FirstOrDefault(t => t.gameObject.name == UIConfig.Canvas);
            return canvas ? canvas.gameObject : null;
        }

        private static IElement CreateElement(string uid, ElementData data,
            Vector2 referenceResolution, bool isDevMode)
        {
            var factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
            var factory = factoryProvider.GetFactory(data.type);
            var parentElement = GetParentFromElement(data.parent);
            var element = factory.Create(uid, data, parentElement, isDevMode ? null : CreateZIndexContainer(data.zIndex).transform);
            element.Visible = data.visible;
            return element;
        }

        private static IElement GetParentFromElement(string id)
        {
            // todo: Element 의 Parent 가 Canvas(최상위) 인 경우 Parent 의 UID 를 가지고 있어야 할지?
            return string.IsNullOrEmpty(id) ? _tempCanvas : GetElement(id);
        }

        private static IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }
        
        private static UnityEngine.Canvas CreateZIndexContainer(uint zIndex)
        {
            if (_zIndexContainer.TryGetValue(zIndex, out var container))
            {
                return container;
            }
            else
            {
                string goName = $"Z-Index-[{zIndex}]";
                GameObject go = new GameObject(goName);
                go.layer = LayerMask.NameToLayer(UIConfig.LayerName);
                
                // set canvas sorting order
                var canvas = go.AddComponent<UnityEngine.Canvas>();
                canvas.sortingOrder = (int)zIndex;
                
                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.SetParent(_tempCanvas.Self);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;
                rectTransform.SetSiblingIndex((int)zIndex);
                _zIndexContainer.Add(zIndex, canvas);
                return canvas;
            }
        }
    }
}