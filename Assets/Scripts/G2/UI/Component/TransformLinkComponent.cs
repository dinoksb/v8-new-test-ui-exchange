using UnityEngine;
using G2.UI.Elements;
using Utilities;

namespace G2.UI
{
    [ExecuteInEditMode]
    public class TransformLinkComponent : MonoBehaviour
    {
        public RectTransform Self
        {
            get => _rectTransform;
        }

        internal RectTransform Target
        {
            get => _targetRectTransform;
        }
        
        private RectTransform _targetRectTransform;
        private RectTransform _rectTransform;
        

        public void Initialize(RectTransform target)
        {
            _targetRectTransform = target;
            InternalDebug.Assert(target, "target RectTransform must not be null.");
            
            _rectTransform ??= GetComponent<RectTransform>();
            InternalDebug.Assert(_rectTransform, "RectTransform must not be null.");
            
            _rectTransform.anchorMin = _targetRectTransform.anchorMin;
            _rectTransform.anchorMax = _targetRectTransform.anchorMax;
            _rectTransform.pivot = _targetRectTransform.pivot;

            _rectTransform.position = _targetRectTransform.position;
            _rectTransform.rotation = _targetRectTransform.rotation;
            _rectTransform.localScale = _targetRectTransform.localScale;
            _rectTransform.sizeDelta = _targetRectTransform.sizeDelta;
        }

        public void SetVisible(IElement element)
        {
            InternalDebug.Log($"Set visible from {element.Name} to {_rectTransform.name}: {element.Visible}");
            _rectTransform.gameObject.SetActive(element.Visible);
        }

        public void SetPosition(IElement element)
        {
            InternalDebug.Log($"Set position from {element.Name} to {_rectTransform.name}: {element.Position}");
            _rectTransform.position = element.Self.position;
        }
        
        public void SetRotation(IElement element)
        {
            InternalDebug.Log($"Set rotation from {element.Name} to {_rectTransform.name}: {element.Rotation}");
            _rectTransform.rotation = element.Self.rotation;
            _rectTransform.position = element.Self.position;
        }

        public void SetSize(IElement element)
        {
            InternalDebug.Log($"Set size from {element.Name} to {_rectTransform.name}: {element.Size}");
            _rectTransform.sizeDelta = element.Self.sizeDelta;
        }
    }
}