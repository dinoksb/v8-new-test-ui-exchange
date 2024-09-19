using G2.UI.Elements;
using UnityEngine;

namespace G2.UI
{
    public class FrameComponents : ElementComponents
    {
        public TransformLinkComponent TransformLinkComponent { get; }
        
        public FrameComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            var rectTransform = Self;
            if (zIndexParent)
            {
                rectTransform = CreateUIElement(name, zIndexParent, false);
                TransformLinkComponent = rectTransform.gameObject.AddComponent<TransformLinkComponent>();
            }
        }
    }
}
