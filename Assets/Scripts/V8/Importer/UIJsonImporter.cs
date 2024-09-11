using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using V8.Utilities;

namespace V8
{
    public static class UIJsonImporter
    {
        private static IElement _tempCanvas;
        private static Dictionary<string, IElement> _ui = new();
        private static Dictionary<string, Sprite> _sprites = new();

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
            if (_sprites.Count == 0 || _ui.Count == 0) return;
     
            foreach (var (_, sprite) in _sprites)
            {
                Object.DestroyImmediate(sprite);
            }
            
            GameObject rootUIObject = _tempCanvas?.Self ? _tempCanvas.Self.gameObject : FindRootObject();
            if (rootUIObject)
            {
                Object.DestroyImmediate(rootUIObject);
                _tempCanvas = null;
            }
            
            _sprites.Clear();
            _ui.Clear();
            InternalDebug.Log("[UIJsonImporter] ui element released");
        }

        public static void ImportAndBuild(string filePath)
        {
            Release();
            var json = File.ReadAllText(filePath);
            UIData? uiData = Import(json);
            if (uiData == null)
            {
                InternalDebug.LogError("Error: ui json load failed");
                return;
            };
            BuildUI(uiData);
        }

        private static async void BuildUI(UIData? data)
        {
            var studio = data.Value.studioData;
            var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            var asset = data.Value.asset;
            var ui = data.Value.ui;

            _tempCanvas = new Canvas(UIConfig.Canvas, null, referenceResolution, false);
            _sprites = await SpriteImporter.Import(asset.texture, asset.sprite, Application.persistentDataPath, true);

            foreach (var (key, element) in ui)
            {
                if (_ui.ContainsKey(key)) continue;
                var factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
                var factory = factoryProvider.GetFactory(element.type);
                var createdElement = CreateElement(key, element, factory, referenceResolution);
                _ui.Add(key, createdElement);
            }
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

        private static IElement CreateElement(string uid, ElementData data, IElementFactory<Element> factory,
            Vector2 referenceResolution)
        {
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(uid, data, parent);
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
    }
}