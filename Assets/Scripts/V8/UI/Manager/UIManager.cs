using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using V8.Utilities;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        [Serializable]
        public class RectTransformWithZIndex
        {
            public RectTransform RectTransform; // Element 의 RectTransform 값
            public uint ZIndex;                 // Element 의 z-index 값
            public int CreationOrder;           // Element 의 생성 순서를 나타내는 값

            public RectTransformWithZIndex(RectTransform rectTransform, uint zIndex, int creationOrder)
            {
                RectTransform = rectTransform;
                ZIndex = zIndex;
                CreationOrder = creationOrder;
            }
        }
        
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        private Dictionary<string, IElement> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        private List<RectTransformWithZIndex> _elementsWithZIndex = new();

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

                _sprites = await SpriteImporter.Import(asset.resource, asset.sprite, Application.persistentDataPath,
                    true);
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
            SortByZIndex();
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
            if (_elementsWithZIndex.Count == 0) return;
            
            // MoveToFront의 z-index 값을 찾기
            var target = _elementsWithZIndex.FirstOrDefault(compareElement => compareElement.RectTransform == element.Self);
            if (target == null)
            {
                InternalDebug.LogError($"[{element.Name}] element does not exist in the list.");
                return;
            }

            // 동일한 z-index를 가진 오브젝트들 중 가장 높은 hierarchy sibling index 를 찾음
            var highestSiblingIndex = _elementsWithZIndex
                .Where(compareElement => compareElement.ZIndex == target.ZIndex)
                .Max(compareElement => compareElement.RectTransform.GetSiblingIndex());

            // 선택한 오브젝트를 가장 높은 hierarchy sibling index로 설정
            target.RectTransform.SetSiblingIndex(highestSiblingIndex);
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
            _elementsWithZIndex.Clear();
            Destroy(_dontDestoryCanvas.Self?.gameObject);
            _dontDestoryCanvas = null;
            Resources.UnloadUnusedAssets();
            InternalDebug.Log("ui element released");
        }

        private void BuildUI(Dictionary<string, ElementData> uis, Vector2 referenceResolution)
        {
            int index = 0;
            foreach (var (key, element) in uis)
            {
                if (_ui.ContainsKey(key)) continue;

                _ui.Add(key, CreateElement(index, key, element, referenceResolution));
                index++;
            }
        }

        private IElement CreateElement(int creationOrder, string uid, ElementData data, Vector2 referenceResolution)
        {
            var frameData = (FrameData)data;
            var dimOpacity = frameData.dim;


            _factoryProvider = new ElementFactoryProvider(_sprites, referenceResolution, dimOpacity, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var parent = GetParentFromElement(data.parent);
            var element = factory.Create(uid, data, parent);
            element.Visible = data.visible;
            _elementsWithZIndex.Add(new RectTransformWithZIndex(element.Self, element.ZIndex, creationOrder));
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

        private void SortByZIndex()
        {
            // z-index 값으로 오름차순 정렬
            // z-index 값이 같을 경우, creationOrder를 기준으로 오름차순 정렬
            _elementsWithZIndex.Sort((elementA, elementB) =>
            {
                int zIndexComparison = elementA.ZIndex.CompareTo(elementB.ZIndex);
                if (zIndexComparison == 0)
                {
                    // z-index가 같으면 생성 순서로 비교
                    return elementA.CreationOrder.CompareTo(elementB.CreationOrder);
                }
                return zIndexComparison;
            });
            
            // 정렬된 순서대로 hierarchy sibling index 설정
            int elementsCount = _elementsWithZIndex.Count;
            for (int i = 0; i < elementsCount; ++i)
            {
                _elementsWithZIndex[i].RectTransform.SetSiblingIndex(i);
            }
        }
    }
}