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
            go.layer = LayerMask.NameToLayer(UIConfig.LayerName);
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
    }
}