using G2.UI.Elements;
using UnityEngine;

namespace G2.UI.Component
{
    public class UpdatableElementComponents : ElementComponents
    {
        public TransformLinkComponent ElementTransformLinkComponent { get; }
        protected RectTransform ZIndexRectTransform { get; }
        
        public UpdatableElementComponents(IElement parent, Transform parentTransform, Transform zIndexParent, string name) : base(parent, parentTransform, zIndexParent, name)
        {
            if (!zIndexParent) return;
            
            ZIndexRectTransform = CreateUIElement(name, zIndexParent, false);
            ElementTransformLinkComponent = ZIndexRectTransform.gameObject.AddComponent<TransformLinkComponent>();
        }
    }
}
