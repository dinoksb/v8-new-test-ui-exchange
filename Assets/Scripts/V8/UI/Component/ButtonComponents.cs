using UnityEngine.EventSystems;

namespace V8
{
    public class ButtonComponents : FrameComponents
    {
        public EventTrigger EventTrigger { get; }

        public ButtonComponents(IElement parent, string id) : base(parent, id)
        {
            EventTrigger = Self.gameObject.AddComponent<EventTrigger>();
        }
    }
}