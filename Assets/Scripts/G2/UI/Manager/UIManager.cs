using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using G2.Converter;
using G2.Model.UI;
using G2.UI;
using G2.UI.Elements;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace G2.Manager
{
    public class UIManager : MonoBehaviour
    {
        private const string _ROOT_DIRECTORY = "UI";
        private const string _INITIALIZE_UI_CANVAS = "INITIALIZE_UI_CANVAS";

        private JsonSerializerSettings _jsonSerializerSettings;
        private string _verseAddress;

        private readonly Dictionary<string, IElement> _elementsByUid = new();
        private readonly Dictionary<string, List<IElement>> _elementsByName = new();
        private readonly Dictionary<string, Sprite> _sprites = new();
        private readonly Dictionary<uint, Canvas> _zIndexContainer = new();
        private readonly List<IElement> _visibleElements = new();
        private Canvas _initializeCanvas;

        private IElement _currentFrontElement;

        private string RootPath => Path.Combine(Application.persistentDataPath, _verseAddress, _ROOT_DIRECTORY);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        public IReadOnlyDictionary<string, IElement> GetElements()
        {
            return _elementsByUid;
        }
        
        private void Awake()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>(ElementDataConverter.Converters)
            };
        }

        public async UniTask<bool> LoadAsync(string verseAddress, string json, bool forceResourceDownload = false, CancellationToken cancellationToken = default)
        {
            Release();
            try
            {
                _verseAddress = verseAddress;
                UIData uiData;
                try
                {
                    uiData = JsonConvert.DeserializeObject<UIData>(json, _jsonSerializerSettings);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to deserialize ui data. Error: {e.Message}");
                    return false;
                }

                // Download resources
                var textureDownloadTasks = Enumerable.Select(uiData.Textures, keyValue =>
                    ResourceDownloader.DownloadTexture(
                        keyValue.Value.Url,
                        RootPath,
                        wantFileName: keyValue.Key,
                        forceDownload: forceResourceDownload,
                        cancellationToken: cancellationToken)).ToList();

                var textureMap = new Dictionary<string, string>();
                try
                {
                    var textureDownloadedFilePaths = await UniTask.WhenAll(textureDownloadTasks);
                    var index = 0;
                    foreach (var keyValue in uiData.Textures)
                    {
                        textureMap[keyValue.Key] = textureDownloadedFilePaths[index];
                        ++index;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }

                foreach (var (key, spriteSheet) in uiData.SpriteSheets)
                {
                    var mangedKey = $"{_ROOT_DIRECTORY}:{key}";
                    var sprite = SpriteManager.GetSprite(mangedKey);
                    if (sprite == null)
                    {
                        var offset = TypeConverter.ToVector2(spriteSheet.Offset);
                        var size = TypeConverter.ToVector2(spriteSheet.CellSize);
                        var border = TypeConverter.ToVector4(spriteSheet.Border);
                        var pivot = TypeConverter.ToVector2(spriteSheet.Pivot, TypeConverter.Vector2Center);

                        var texture = await TextureManager.LoadTextureAsync(textureMap[spriteSheet.TextureId], cancellationToken);

                        sprite = SpriteManager.CreateSprite(
                            key: mangedKey,
                            texture: texture,
                            offset: offset,
                            size: size,
                            border: border,
                            pivot: pivot,
                            pixelsPerUnit: spriteSheet.PixelsPerUnit
                        );
                    }

                    _sprites.Add(key, sprite);
                }


                var referenceResolution = new Vector2(uiData.StudioData.resolutionWidth, uiData.StudioData.resolutionHeight);
                BuildUI(uiData.UI, referenceResolution);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public void Show(Canvas target)
        {
            var childCount = _initializeCanvas.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                _initializeCanvas.transform.GetChild(0).SetParent(target.transform);
            }

            foreach (var (_, canvas) in _zIndexContainer)
            {
                canvas.overrideSorting = true;
            }
        }

        public IElement Get(string uid)
        {
            if (_elementsByUid.TryGetValue(uid, out var element))
            {
                return element;
            }

            Debug.Log($"[{uid}]: element does not exist.");
            return null;
        }

        public T Get<T>(string uid) where T : class, IElement
        {
            var element = Get(uid);
            if (element is T typedElement)
            {
                return typedElement;
            }
            
            Debug.Log($"[{uid}]: element exists but is not of type {typeof(T).Name}.");
            return null;
        }

        public IReadOnlyList<IElement> GetFromName(string findName)
        {
            return _elementsByName.TryGetValue(findName, out var elements) ? elements : null;
        }

        // A function that sets a specific object to the top among objects with the same z-index.
        public void MoveToFront(IElement element)
        {
            _currentFrontElement = element;
            _currentFrontElement.MoveFront();
        }

        public IElement GetFrontFrame()
        {
            if (_currentFrontElement is { Visible: true })
            {
                return GetRootElement(_currentFrontElement);
            }

            var highestZIndex = uint.MinValue;
            IElement elementWithHighestZIndex = null;

            foreach (var element in _visibleElements)
            {
                if (element.ZIndex < highestZIndex) continue;
                highestZIndex = element.ZIndex;
                elementWithHighestZIndex = element;
            }

            return GetRootElement(elementWithHighestZIndex);
        }

        public void Delete(string uid)
        {
            if (_elementsByUid == null) return;
            if (_elementsByUid.Count == 0) return;

            _elementsByUid.Remove(uid);
            Debug.Log($"[{uid}]: element deleted.");
        }

        public void Release()
        {
            foreach (var (_, sprite) in _sprites)
            {
                Destroy(sprite);
            }

            foreach (var (_, ui) in _elementsByUid)
            {
                if (ui.Self) Destroy(ui.Self.gameObject);
            }

            _sprites.Clear();
            _elementsByUid.Clear();
            _zIndexContainer.Clear();
            _visibleElements.Clear();
            if (_initializeCanvas != null && _initializeCanvas.transform != null)
            {
                Destroy(_initializeCanvas.transform.gameObject);
            }

            _initializeCanvas = null;
            Resources.UnloadUnusedAssets();
            Debug.Log("ui element released");
        }

        private void BuildUI(Dictionary<string, ElementData> uis, Vector2 referenceResolution)
        {
            _initializeCanvas ??= CreateCanvas(referenceResolution);
            foreach (var (key, element) in uis)
            {
                if (_elementsByUid.ContainsKey(key)) continue;
                var instance = CreateElement(key, element, referenceResolution);
                _elementsByUid.Add(key, instance);
                var elementName = element.name;
                if (!_elementsByName.ContainsKey(elementName))
                {
                    _elementsByName.Add(elementName, new List<IElement>());
                }
                _elementsByName[elementName].Add(instance);
            }
        }

        private IElement CreateElement(string uid, ElementData data, Vector2 referenceResolution)
        {
            if (!Enum.TryParse(data.type, ignoreCase: true, out ElementType type)) throw new ArgumentOutOfRangeException($"Invalid type: {data.type}");
            
            var parentElement = GetParentFromElement(data.parent);
            var parentTransform = parentElement == null ? _initializeCanvas.transform : parentElement.Self;
            var zIndexParent = CreateZIndexContainer(data.zIndex);
            IElement element;
            switch (type)
            {
                case ElementType.Frame:
                    element = ElementFactory.CreateFrame(uid, parentElement, zIndexParent, (FrameData)data);
                    break;
                case ElementType.Image:
                    var imageData = (ImageData)data;
                    if (!_sprites.TryGetValue(imageData.spriteId, out var sprite)) throw new KeyNotFoundException($"Sprite with ID {imageData.spriteId} not found in the dictionary.");
                    element = ElementFactory.CreateImage(uid, parentElement, zIndexParent, imageData, sprite);
                    break;
                case ElementType.Label:
                    element = ElementFactory.CreateLabel(uid, parentElement, zIndexParent, (LabelData)data, referenceResolution);
                    break;
                case ElementType.Button:
                    element = ElementFactory.CreateButton(uid, parentElement, zIndexParent, (ButtonData)data, OnEvent);
                    break;
                case ElementType.ScrollFrame:
                    element = ElementFactory.CreateScrollFrame(uid, parentElement, zIndexParent, (ScrollFrameData)data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected ElementType: {type}");
            }
            
            element.AddVisibleChangedListener(OnVisibleChangeListener);
            element.Visible = data.visible;
            return element;
        }
        
        private Canvas CreateCanvas(Vector2 canvasResolution)
        {
            var gameObject = new GameObject(_INITIALIZE_UI_CANVAS);

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = canvasResolution;
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();
            gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
            return canvas;
        }

        private IElement GetParentFromElement(string id)
        {
            return string.IsNullOrEmpty(id) ? null : GetElement(id);
        }

        private IElement GetElement(string id)
        {
            return _elementsByUid.GetValueOrDefault(id);
        }

        private Transform CreateZIndexContainer(uint zIndex)
        {
            if (_zIndexContainer.TryGetValue(zIndex, out var container))
            {
                return container.transform;
            }

            var goName = $"Z-Index-[{zIndex}]";
            var go = new GameObject(goName)
            {
                layer = LayerMask.NameToLayer(Config.LayerName.UI)
            };

            // set canvas sorting order
            var canvas = go.AddComponent<Canvas>();
            canvas.sortingOrder = (int)zIndex;

            // add graphicRaycaster
            go.AddComponent<GraphicRaycaster>();

            // set rectTransform values
            var rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.SetParent(_initializeCanvas.transform);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            _zIndexContainer.Add(zIndex, canvas);
            return rectTransform;
        }

        private void OnVisibleChangeListener(IElement element)
        {
            var isVisible = element.Visible;

            if (isVisible)
            {
                if (!_visibleElements.Contains(element))
                    _visibleElements.Add(element);
            }
            else
            {
                _visibleElements.Remove(element);
            }
        }

        private static IElement GetRootElement(IElement element)
        {
            while (true)
            {
                if (element.Parent.Type == nameof(Canvas)) return element;
                element = element.Parent;
            }
        }
    }
}
