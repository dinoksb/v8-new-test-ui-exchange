using G2.UI.Elements;
using UnityEngine;

namespace G2.UI
{
    public class FrameComponents : ElementComponents
    {
        public TransformLinkComponent SourceTransformLinkComponent { get; }
        public TransformLinkComponent GUITransformLinkComponent { get; }
        public RectTransform GUIRectTransform { get; }
        
        public FrameComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
        }
        
        public FrameComponents(IElement parent, Transform zIndexParent, ElementType type, string name) : base(parent, zIndexParent, name)
        {
            var rectTransform = Self;
            if (zIndexParent && type == ElementType.Frame)
            {
                GUIRectTransform = CreateUIElement(name, zIndexParent, false);
                GUITransformLinkComponent = GUIRectTransform.gameObject.AddComponent<TransformLinkComponent>();
                SourceTransformLinkComponent = rectTransform.gameObject.AddComponent<TransformLinkComponent>();
            }
        }
    }
}
