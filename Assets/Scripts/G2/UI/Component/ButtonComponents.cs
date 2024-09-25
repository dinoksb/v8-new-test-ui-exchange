using G2.UI.Elements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace G2.UI.Component
{
    public class ButtonComponents : UpdatableElementComponents
    {
        public EventTrigger EventTrigger { get; }
        public NonDrawingGraphic NonDrawingGraphic { get; }
        
        public ButtonComponents(IElement parent, Transform parentTransform, Transform zIndexParent, string name) : base(parent, parentTransform, zIndexParent, name)
        {
            RectTransform buttonRectTransform = ZIndexRectTransform;
            EventTrigger = buttonRectTransform.gameObject.AddComponent<EventTrigger>();
            NonDrawingGraphic = buttonRectTransform.gameObject.AddComponent<NonDrawingGraphic>();
        }
    }
}
