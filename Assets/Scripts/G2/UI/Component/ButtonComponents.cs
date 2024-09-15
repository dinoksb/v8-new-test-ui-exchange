using UnityEngine;
using UnityEngine.EventSystems;
using G2.UI.Elements;

namespace G2.UI
{
    public class ButtonComponents : FrameComponents
    {
        public EventTrigger EventTrigger { get; }
        public TransformLinkComponent TransformLinkComponent { get; }
        public NonDrawingGraphic NonDrawingGraphic { get; }

        public ButtonComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            RectTransform buttonRectTransform = Self;
            if (zIndexParent)
            {
                buttonRectTransform = CreateUIElement(Self.name, zIndexParent, false);
            }
            TransformLinkComponent = buttonRectTransform.gameObject.AddComponent<TransformLinkComponent>();
            EventTrigger = buttonRectTransform.gameObject.AddComponent<EventTrigger>();
            NonDrawingGraphic = buttonRectTransform.gameObject.AddComponent<NonDrawingGraphic>();
        }
    }
}