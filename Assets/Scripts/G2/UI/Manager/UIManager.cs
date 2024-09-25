using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using G2.Converter;
using G2.Model.UI;
using G2.UI;
using G2.UI.Component;
using G2.UI.Elements;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace G2.Manager
{
    public class UIManager : MonoBehaviour
    {
        private class ElementWithTransformLink
        {
            public IElement Element;
            public TransformLinkComponent TransformLinkComponent;

            public ElementWithTransformLink(IElement element, TransformLinkComponent transformLinkComponent)
            {
                Element = element;
                TransformLinkComponent = transformLinkComponent;
            }
        }

        private const string _ROOT_DIRECTORY = "UI";
        private const string _ROOT_GAME_OBJECT_NAME = "UI";

        private JsonSerializerSettings _jsonSerializerSettings;
        private string _verseAddress;

        private readonly Dictionary<string, IElement> _elementsByUid = new();
        private readonly Dictionary<string, List<IElement>> _elementsByName = new();
        private readonly Dictionary<string, Sprite> _sprites = new();
        private readonly Dictionary<uint, Canvas> _zIndexCanvases = new();
        private readonly List<IElement> _visibleElements = new();
        private readonly List<ElementWithTransformLink> _rootFrames = new();
        private GameObject _canvasGameObject;


        private IElement _cachedMoveFrontElement;

        private string RootPath => Path.Combine(Application.persistentDataPath, _verseAddress, _ROOT_DIRECTORY);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        private void Awake()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>(ElementDataConverter.Converters)
            };
        }

        #region IUIManager

        public IReadOnlyDictionary<string, IElement> GetElements()
        {
            return _elementsByUid;
        }

        public async UniTask<bool> LoadAsync(string verseAddress, string json, bool forceResourceDownload = false,
            CancellationToken cancellationToken = default)
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
                        var pivot = TypeConverter.ToVector2(spriteSheet.Pivot, TypeConverter.Vector2Center)
                            .ToReverseYAxis();
#if UNITY_WEBGL && !UNITY_EDITOR
                        var texture = TextureManager.LoadTexture(textureMap[spriteSheet.TextureId]);
#else
                        var texture =
                            await TextureManager.LoadTextureAsync(textureMap[spriteSheet.TextureId], cancellationToken);
#endif
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

                var referenceResolution =
                    new Vector2(uiData.StudioData.resolutionWidth, uiData.StudioData.resolutionHeight);
                BuildUI(uiData.UI, referenceResolution);
                Debug.Log("[UIManager] Build UI Complete");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public void Show(Transform parent = null)
        {
            if (_canvasGameObject == null) return;

            _canvasGameObject.SetActive(true);
            if (parent == null)
            {
                _canvasGameObject.transform.SetParent(null, false);
                SceneManager.MoveGameObjectToScene(_canvasGameObject, SceneManager.GetActiveScene());
            }
            else
            {
                _canvasGameObject.transform.SetParent(parent, false);
                _canvasGameObject = parent.gameObject;
            }

            foreach (var (_, canvas) in _zIndexCanvases)
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
            if (!element.Self) return;
            if (_cachedMoveFrontElement == element) return;

            var visualTransformLinkCompoenents = element.Self.GetComponentsInChildren<TransformLinkComponent>();
            foreach (var visualTransform in visualTransformLinkCompoenents)
            {
                visualTransform.Target.SetAsLastSibling();
            }

            _cachedMoveFrontElement = element;
        }

        // Todo: If the user generates UI Data without a Frame Type, there is a potential for issues. Verification is required.
        public IElement GetFrontFrame()
        {
            if (_rootFrames.Count == 0)
            {
                Debug.Log("[UIManager] GetFrontFrame: Frame element does not exist!!");
                return null;
            }

            IElement highestElement = null;
            int highestIndex = -1;

            foreach (var frameData in _rootFrames)
            {
                if (!frameData.Element.Visible) continue;

                int siblingIndex = frameData.TransformLinkComponent.Target.GetSiblingIndex();
                if (siblingIndex > highestIndex)
                {
                    highestIndex = siblingIndex;
                    highestElement = frameData.Element;
                }
            }

            return highestElement;
        }

        public void Release()
        {
            foreach (var (_, sprite) in _sprites)
            {
                SpriteManager.OnSpriteUnused(sprite);
            }

            foreach (var (_, element) in _elementsByUid)
            {
                if (element.Self) Destroy(element.Self.gameObject);
            }

            _sprites.Clear();
            _elementsByUid.Clear();
            _elementsByName.Clear();
            _zIndexCanvases.Clear();
            _visibleElements.Clear();
            if (_canvasGameObject != null)
            {
                Destroy(_canvasGameObject);
                _canvasGameObject = null;
            }

            Resources.UnloadUnusedAssets();
        }

        #endregion

        private void BuildUI(Dictionary<string, ElementData> uis, Vector2 referenceResolution)
        {
            if (_canvasGameObject == null)
            {
                _canvasGameObject = CreateCanvasGameObject(referenceResolution);
                _canvasGameObject.transform.SetParent(gameObject.transform, false);
            }

            foreach (var (key, element) in uis)
            {
                if (_elementsByUid.ContainsKey(key)) continue;
                if (!Enum.TryParse(element.Type, ignoreCase: true, out ElementType type))
                    throw new ArgumentOutOfRangeException($"Invalid type: {element.Type}");

                var instance = CreateElement(key, element, type, referenceResolution);
                _elementsByUid.Add(key, instance);
                var elementName = element.Name;
                if (!_elementsByName.ContainsKey(elementName))
                {
                    _elementsByName.Add(elementName, new List<IElement>());
                }

                _elementsByName[elementName].Add(instance);

                // Only the top-level elements of the Frame type are added.
                if (type == ElementType.Frame && instance.Parent == null)
                {
                    var transformLink = instance.Self.GetComponent<TransformLinkComponent>();
                    _rootFrames.Add(new ElementWithTransformLink(instance, transformLink));
                }
            }

            _rootFrames.Sort((elementX, elementY) => elementY.Element.ZIndex.CompareTo(elementX.Element.ZIndex));
        }

        private IElement CreateElement(string uid, ElementData data, ElementType type, Vector2 referenceResolution)
        {
            var parentElement = GetParentFromElement(data.Parent);
            var parentTransform = parentElement == null ? _canvasGameObject.transform : parentElement.Self;
            var zIndexCanvasTransform = CreateZIndexCanvas(data.ZIndex);
            IElement element;
            switch (type)
            {
                case ElementType.Frame:
                    element = ElementFactory.CreateFrame(uid, parentElement, parentTransform, zIndexCanvasTransform,
                        (FrameData)data);
                    break;
                case ElementType.Image:
                    var imageData = (ImageData)data;
                    if (!_sprites.TryGetValue(imageData.spriteId, out var sprite))
                        throw new KeyNotFoundException(
                            $"Sprite with ID {imageData.spriteId} not found in the dictionary.");
                    element = ElementFactory.CreateImage(uid, parentElement, parentTransform, zIndexCanvasTransform,
                        imageData, sprite);
                    break;
                case ElementType.Label:
                    element = ElementFactory.CreateLabel(uid, parentElement, parentTransform, zIndexCanvasTransform,
                        (LabelData)data, referenceResolution);
                    break;
                case ElementType.Button:
                    element = ElementFactory.CreateButton(uid, parentElement, parentTransform, zIndexCanvasTransform,
                        (ButtonData)data, OnEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected ElementType: {type}");
            }

            element.Visible = data.Visible;
            return element;
        }

        private static GameObject CreateCanvasGameObject(Vector2 canvasResolution)
        {
            var go = new GameObject(_ROOT_GAME_OBJECT_NAME);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var canvasScaler = go.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            go.SetActive(false);
            return go;
        }

        private IElement GetParentFromElement(string id)
        {
            return string.IsNullOrEmpty(id) ? null : GetElement(id);
        }

        private IElement GetElement(string id)
        {
            return _elementsByUid.GetValueOrDefault(id);
        }

        private Transform CreateZIndexCanvas(uint zIndex)
        {
            if (_zIndexCanvases.TryGetValue(zIndex, out var container))
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

            go.AddComponent<CanvasScaler>();

            // set rectTransform values
            var rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.SetParent(_canvasGameObject.transform);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            _zIndexCanvases.Add(zIndex, canvas);
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
