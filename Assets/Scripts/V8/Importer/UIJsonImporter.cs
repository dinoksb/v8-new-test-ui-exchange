using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using V8.Utilities;

namespace V8
{
    public static class UIJsonImporter
    {
        private static IElement _canvas;
        private static Dictionary<string, IElement> _ui = new();
        private static Dictionary<string, Sprite> _sprites = new();
        
        private static void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }
        
        public static UIData? Import(string json)
        {
            if (IsValidation(json))
            {
                try
                {
                    return JsonConvert.DeserializeObject<UIData>(json, ElementDataConverter.SerializerSettings);
                }
                catch (JsonException e)
                {
                    InternalDebug.LogException(e);
                }
            }
            else
            {
                InternalDebug.LogError("[UIImporter] json format is invalid.");
            }
            return null;
        }

        private static bool IsValidation(string json)
        {
            // TODO: Json Schema 를 통한 유효성 검사
            return true;
        }
        
        public static void ImportAndBuild(string filePath)
        {
            var json = File.ReadAllText(filePath);
            UIData? uiData = Import(json);
            BuildUI(uiData);
        }

        public static void Clear()
        {
            if (_canvas == null && (_sprites.Count == 0 || _ui.Count == 0)) return;
            
            Object.DestroyImmediate(_canvas?.Self.gameObject);
            foreach (var (_, sprite) in _sprites)
            {
                Object.DestroyImmediate(sprite);
            }
            _sprites.Clear();
            _ui.Clear();
            _canvas = null;
            InternalDebug.Log("created ui cleared");
        }

        public static async void BuildUI(UIData? data)
        {
            var studio = data.Value.studioData;
            var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            var asset = data.Value.asset;
            var ui = data.Value.ui;
            
            _canvas = new Canvas(UIConfig.Canvas, null, referenceResolution);
            _sprites = await SpriteImporter.Import(asset.sprite, true);
            
            foreach (var (key, element) in ui)
            {
                if (_ui.ContainsKey(key)) continue;
                var factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
                var factory = factoryProvider.GetFactory(element.type);
                var createdElement = CreateElement(element, factory, referenceResolution);
                _ui.Add(key, createdElement);
            }
        }
        
        private static IElement CreateElement(ElementData data, IElementFactory<Element> factory, Vector2 referenceResolution)
        {
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(data, parent);
            element.Visible = data.visible;
            return element;
        }
        
        private static IElement GetParentFromElement(string id)
        {
            return GetElement(id) ?? _canvas;
        }
        
        private static IElement GetElement(string id)
        {
            return _ui.GetValueOrDefault(id);
        }
    }
}

