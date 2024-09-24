using G2.UI.Elements;
using UnityEngine;

namespace G2.UI.Component
{
    public class ElementComponents
    {
        public IElement Parent { get; }
        public TransformLinkComponent VisualTransformLinkComponent { get; }
        public RectTransform ParentRectTransform { get; }
        public RectTransform Self { get; }
        
        public ElementComponents(IElement parentElement, Transform parentTransform, Transform zIndexParent, string name)
        {
            var self = CreateUIElement(name, parentTransform, false);
            Parent = parentElement;
            ParentRectTransform = parentTransform as RectTransform;
            Self = self;
            VisualTransformLinkComponent = self.gameObject.AddComponent<TransformLinkComponent>();
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
    }
}
