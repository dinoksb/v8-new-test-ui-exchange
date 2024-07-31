using System.Collections.Generic;
using System.Linq;

namespace V8
{
    public sealed class UIEvent
    {
        private readonly List<EventAction> _eventActions = new();
        
        public delegate void EventAction(ulong clientId, string uiId, string eventId);

        public void Invoke(ulong clientId, string uiId, string eventId)
        {
            var copiedEventActions = _eventActions.ToList();
            foreach (var eventAction in copiedEventActions)
            {
                eventAction.Invoke(clientId, uiId, eventId);
            }
        }

        public void Add(EventAction eventAction)
        {
            if (!_eventActions.Contains(eventAction))
            {
                _eventActions.Add(eventAction);
            }
        }

        public void Remove(EventAction eventAction)
        {
            _eventActions.Remove(eventAction);
        }

        public void Clear()
        {
            _eventActions.Clear();
        }
    }
}