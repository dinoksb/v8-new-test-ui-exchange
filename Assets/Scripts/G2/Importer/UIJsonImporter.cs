using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using G2.Converter;
using G2.Model.UI;
using G2.UI;
using G2.UI.Elements;
using Utilities;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

namespace G2.Importer
{
    public static class UIJsonImporter
    {
        private const string _CANVAS_NAME = "UICanvas";
        
        private static IElement _tempCanvas;
        private static Dictionary<string, IElement> _ui = new();
        private static Dictionary<string, Sprite> _sprites = new();
        private static Dictionary<uint, Canvas> _zIndexContainer = new();

        private static void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        public static UIData? Import(string url)
        {
            if (IsValidation(url))
            {
                try
                {
                    var jsonSerializerSettings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter>(ElementDataConverter.Converters)
                    };
                    return JsonConvert.DeserializeObject<UIData>(url, jsonSerializerSettings);
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
            var studio = data.Value.StudioData;
            var referenceResolution = new Vector2(studio.resolutionWidth, studio.resolutionHeight);
            var textures = data.Value.Textures;
            var spriteSheets = data.Value.SpriteSheets;
            var ui = data.Value.UI;

            _tempCanvas = new G2.UI.Elements.Basic.Canvas(_CANVAS_NAME, null, referenceResolution, false);
            _sprites = await SpriteImporter.Import(textures, spriteSheets, Application.persistentDataPath, true);

            foreach (var (key, element) in ui)
            {
                if (_ui.ContainsKey(key)) continue;
                var createdElement = CreateElement(key, element, referenceResolution, isDevMode);
                if(isDevMode)
                    createdElement.Self.gameObject.AddComponent<DevElementInfo>().Attach(createdElement);
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
            var canvas = canvases.FirstOrDefault(t => t.gameObject.name == _CANVAS_NAME);
            return canvas ? canvas.gameObject : null;
        }

        private static IElement CreateElement(string uid, ElementData data,
            Vector2 referenceResolution, bool isDevMode)
        {
            if (!Enum.TryParse(data.type, ignoreCase: true, out ElementType type)) throw new ArgumentOutOfRangeException($"Invalid type: {data.type}");

            var parent = GetParentFromElement(data.parent);
            var zIndexParent = isDevMode? null : CreateZIndexContainer(data.zIndex).transform;
            IElement element;
            switch (type)
            {
                case ElementType.Frame:
                    element = ElementFactory.CreateFrame(uid, parent, zIndexParent, (FrameData)data);
                    break;
                case ElementType.Image:
                    var imageData = (ImageData)data;
                    if (!_sprites.TryGetValue(imageData.spriteId, out var sprite)) throw new KeyNotFoundException($"Sprite with ID {imageData.spriteId} not found in the dictionary.");
                    element = ElementFactory.CreateImage(uid, parent, zIndexParent, imageData, sprite);
                    break;
                case ElementType.Label:
                    element = ElementFactory.CreateLabel(uid, parent, zIndexParent, (LabelData)data, referenceResolution);
                    break;
                case ElementType.Button:
                    element = ElementFactory.CreateButton(uid, parent, zIndexParent, (ButtonData)data, OnEvent);
                    break;
                case ElementType.ScrollFrame:
                    element = ElementFactory.CreateScrollFrame(uid, parent, zIndexParent, (ScrollFrameData)data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected ElementType: {type}");
            }
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
        
        private static Canvas CreateZIndexContainer(uint zIndex)
        {
            if (_zIndexContainer.TryGetValue(zIndex, out var container))
            {
                return container;
            }
            else
            {
                string goName = $"Z-Index-[{zIndex}]";
                GameObject go = new GameObject(goName);
                go.layer = LayerMask.NameToLayer(Config.LayerName.UI);
                
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
