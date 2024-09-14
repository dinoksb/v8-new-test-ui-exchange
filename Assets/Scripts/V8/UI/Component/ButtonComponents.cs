using UnityEngine;
using UnityEngine.EventSystems;

namespace V8
{
    public class ButtonComponents : FrameComponents
    {
        public EventTrigger EventTrigger { get; }
        public TransformLinkComponents TransformLinkComponents { get; }

        public ButtonComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            var eventTriggerSource = new GameObject(Self.name);
            var eventTriggerSourceRectTransform = eventTriggerSource.AddComponent<RectTransform>();
            eventTriggerSourceRectTransform.SetParent(zIndexParent);
            eventTriggerSourceRectTransform.localPosition = Vector3.zero;
            eventTriggerSourceRectTransform.localRotation = Quaternion.identity;
            eventTriggerSourceRectTransform.localScale = Vector3.one;
            
            TransformLinkComponents = eventTriggerSource.AddComponent<TransformLinkComponents>();
            EventTrigger = eventTriggerSource.AddComponent<EventTrigger>();
        }
    }
}