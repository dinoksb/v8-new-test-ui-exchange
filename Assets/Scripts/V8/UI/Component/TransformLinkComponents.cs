using System;
using UnityEngine;
using V8.Utilities;

namespace V8
{
    [ExecuteInEditMode]
    public class TransformLinkComponents : MonoBehaviour
    {
        public RectTransform Self
        {
            get => _rectTransform;
        }

        private RectTransform _targetRectTransform;
        private RectTransform _rectTransform;

        public void Initialize(RectTransform targetRectTransform)
        {
            _targetRectTransform = targetRectTransform;
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
            InternalDebug.Log($"Set visible from {element.Name}");
            _rectTransform.gameObject.SetActive(element.Visible);
        }

        public void SetPosition(IElement element)
        {
            InternalDebug.Log($"Set position from {element.Name}");
            _rectTransform.position = element.Self.position;
        }

        public void SetRotation(IElement element)
        {
            InternalDebug.Log($"Set rotation from {element.Name}");
            _rectTransform.rotation = element.Self.rotation;
        }

        public void SetSize(IElement element)
        {
            InternalDebug.Log($"Set size from {element.Name}");
            _rectTransform.sizeDelta = element.Self.sizeDelta;
        }
    }
}