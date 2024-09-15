using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using G2.Importer;
using G2.Model.UI;
using G2.UI;
using G2.UI.Elements;
using G2.UI.Factory;
using G2.UI.Provider;
using Utilities;

namespace G2.Manager
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        private Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        private Dictionary<uint, UnityEngine.Canvas> _zIndexContainer = new();
        private List<IElement> _visibleElements = new();
        private IElement _dontDestoryCanvas;

        private IElement _currentFrontElement;

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

                _sprites = await SpriteImporter.Import(asset.resource, asset.sprite, Application.persistentDataPath,
                    true);
                _dontDestoryCanvas = new G2.UI.Elements.Basic.Canvas(UIConfig.Canvas, null, referenceResolution, true);
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
            int childCount = _dontDestoryCanvas.Self.childCount;

            for (int i = 0; i < childCount; i++)
            {
                _dontDestoryCanvas.Self.GetChild(0).SetParent(target.transform);
            }

            foreach (var (_, canvas) in _zIndexContainer)
            {
                canvas.overrideSorting = true;
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

        // 동일한 z-index를 가진 오브젝트 중 특정 오브젝트를 최상위로 설정하는 함수
        public void MoveToFront(IElement element)
        {
            _currentFrontElement = element;
            _currentFrontElement.MoveFront();
        }

        public IElement GetFrontFrame()
        {
            if (_currentFrontElement != null && _currentFrontElement.Visible)
            {
                return GetRootElement(_currentFrontElement);
            }

            uint highestZIndex = uint.MinValue;
            IElement elementWithHighestZIndex = null;

            foreach (var element in _visibleElements)
            {
                if (element.ZIndex >= highestZIndex)
                {
                    highestZIndex = element.ZIndex;
                    elementWithHighestZIndex = element;
                }
            }

            return GetRootElement(elementWithHighestZIndex);
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
                if (ui.Self)
                    Destroy(ui.Self.gameObject);
            }

            _sprites.Clear();
            _ui.Clear();
            _zIndexContainer.Clear();
            _visibleElements.Clear();
            // _elementsWithZIndex.Clear();
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
            _factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var zIndexParent = CreateZIndexContainer(data.zIndex);
            var element = factory.Create(uid, data, parent, zIndexParent);
            element.AddVisibleChangedListener(OnVisibleChangeListener);
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

        // private void SortByZIndex()
        // {
        //     // z-index 값으로 오름차순 정렬
        //     // z-index 값이 같을 경우, creationOrder를 기준으로 오름차순 정렬
        //     _elementsWithZIndex.Sort((elementA, elementB) =>
        //     {
        //         int zIndexComparison = elementA.ZIndex.CompareTo(elementB.ZIndex);
        //         if (zIndexComparison == 0)
        //         {
        //             // z-index가 같으면 생성 순서로 비교
        //             return elementA.CreationOrder.CompareTo(elementB.CreationOrder);
        //         }
        //         return zIndexComparison;
        //     });
        //     
        //     // 정렬된 순서대로 hierarchy sibling index 설정
        //     int elementsCount = _elementsWithZIndex.Count;
        //     for (int i = 0; i < elementsCount; ++i)
        //     {
        //         _elementsWithZIndex[i].RectTransform.SetSiblingIndex(i);
        //     }
        // }

        private Transform CreateZIndexContainer(uint zIndex)
        {
            if (_zIndexContainer.TryGetValue(zIndex, out var container))
            {
                return container.transform;
            }

            string goName = $"Z-Index-[{zIndex}]";
            GameObject go = new GameObject(goName);
            go.layer = LayerMask.NameToLayer(UIConfig.LayerName);

            // set canvas sorting order
            var canvas = go.AddComponent<UnityEngine.Canvas>();
            canvas.sortingOrder = (int)zIndex;

            // add graphicRaycaster
            go.AddComponent<GraphicRaycaster>();

            // set rectTransform values
            var rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.SetParent(_dontDestoryCanvas.Self);
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
            bool isVisible = element.Visible;

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

        private IElement GetRootElement(IElement element)
        {
            if (element.Parent.Type == nameof(Canvas))
                return element;
            
            return GetRootElement(element.Parent);
        }
    }
}