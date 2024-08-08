using UnityEngine.EventSystems;

namespace V8
{
    public class ButtonComponents : FrameComponents
    {
        public EventTrigger EventTrigger { get; }

        public ButtonComponents(IElement parent) : base(parent)
        {
            EventTrigger = Self.gameObject.AddComponent<EventTrigger>();
        }
    }
}