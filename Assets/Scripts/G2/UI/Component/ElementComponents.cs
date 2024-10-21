using G2.UI.Elements;
using UnityEngine;

namespace G2.UI
{
    public class ElementComponents
    {
        public IElement Parent { get; }
        public RectTransform Self { get; }

        public ElementComponents(IElement parent, Transform zIndexParent, string name)
        {
            var self = CreateUIElement(name, parent.Self, false);
            Parent = parent;
            Self = self;
        }

        protected RectTransform CreateUIElement(string name, Transform parent, bool isStretch)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer(Config.LayerName.UI);
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.SetParent(parent);

            // Optionally set anchors and offsets to stretch the element
            if (isStretch)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

            // reset the transform properties
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        internal RectTransform CreateUIElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Vector2 pivot)
        {
            var rectTransform = InitializeUIElement(name, parent);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
            rectTransform.pivot = pivot;

            // reset the transform properties
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private RectTransform InitializeUIElement(string name, Transform parent)
        {
            var go = new GameObject(name)
            {
                layer = LayerMask.NameToLayer(Config.LayerName.UI)
            };
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.SetParent(parent);

            // reset the transform properties
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            return rectTransform;
        }

        private RectTransform CreateStretchedUIElement(string name, Transform parent)
        {
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;
            Vector2 offset = Vector2.zero;
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return CreateUIElement(name, parent, anchorMin, anchorMax, offset, offset, pivot);
        }

        private RectTransform CreateCenteredUIElement(string name, Transform parent)
        {
            Vector2 anchor = new Vector2(0.5f, 0.5f);
            Vector2 offset = new Vector2(50, 50);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return CreateUIElement(name, parent, anchor, anchor, -offset, offset, pivot);
        }
    }
}
