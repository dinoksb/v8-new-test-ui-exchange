using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace V8
{
    public class Button : Frame
    {
        private EventTrigger _eventTrigger;
        private ReadOnlyDictionary<EventTriggerType, string> _events;
        private readonly Action<ulong, string, string, string> _action;
        // private readonly Dictionary<EventTriggerType, float> _lastEventTimes = new();
        private TransformLinkComponents _transformLink;
        
        private readonly List<Action<IElement>> _pointerEnterEvents = new();
        private readonly List<Action<IElement>> _pointerExitEvents = new();
        private readonly List<Action<IElement>> _pointerDownEvents = new();
        private readonly List<Action<IElement>> _pointerUpEvents = new();

        public ReadOnlyDictionary<EventTriggerType, string> Events
        {
            get => _events;
            private set
            {
                _events = value;
                UpdateEventTrigger(_events);
            }
        }

        // public float Threshold { get; }

        public Button(string uid, ButtonData data, ButtonComponents components, Action<ulong, string, string, string> action)
            : base(uid, data, components)
        {
            _action = action;
            _eventTrigger = components.EventTrigger;
            _transformLink = components.TransformLinkComponents;
            _transformLink.Initialize(Self);
            _visibleChangedActions.Add(_transformLink.SetVisible);
            _positionChangeActions.Add(_transformLink.SetPosition);
            _rotationChangeActions.Add(_transformLink.SetRotation);
            _sizeChangeActions.Add(_transformLink.SetSize);
            // Threshold = data.threshold;

            var e = new Dictionary<EventTriggerType, string>();
            foreach (var (eventType, eventId) in data.events)
            {
                if (TypeConverter.TryEventTriggerType(eventType, out var type))
                {
                    e.Add(type, eventId);
                }
            }

            Events = new ReadOnlyDictionary<EventTriggerType, string>(e);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Button)base.Copy(self, parent);
            clone._eventTrigger = self.GetComponent<EventTrigger>();

            var clonedEvents = new Dictionary<EventTriggerType, string>();
            foreach (var (key, value) in _events)
            {
                clonedEvents.Add(key, value);
            }

            clone._events = new ReadOnlyDictionary<EventTriggerType, string>(clonedEvents);

            clone.UpdateEventTrigger(_events);
            
            _visibleChangedActions.Add(clone._transformLink.SetVisible);
            _positionChangeActions.Add(clone._transformLink.SetPosition);
            _rotationChangeActions.Add(clone._transformLink.SetRotation);
            _sizeChangeActions.Add(clone._transformLink.SetSize);
            return clone;
        }

        public override void MoveFront()
        {
            _transformLink.Self.SetAsLastSibling();
        }

        private void UpdateEventTrigger(ReadOnlyDictionary<EventTriggerType, string> events)
        {
            // todo: 기존에 등록한 이벤트 트리거들 모두 제거됨... 추후 _eventTrigger.triggers.Clear() 로직 제거 필요할수도있음..
            _eventTrigger.triggers.Clear();

            foreach (var (eventType, eventId) in events)
            {
                var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == eventType);
                if (entry != null)
                {
                    _eventTrigger.triggers.Remove(entry);
                }

                _eventTrigger.triggers.Add(CreateEntry(eventType, eventId));
            }
        }

        private EventTrigger.Entry CreateEntry(EventTriggerType type, string eventId)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = type
            };

            // _lastEventTimes[type] = 0;
            entry.callback.AddListener(_ =>
            {
                // var diff = Time.time - _lastEventTimes[type];
                // if (diff < Threshold)
                // {
                //     return;
                // }

                // _lastEventTimes[type] = Time.time;
                //Todo: Network 로 이벤트 보낼 때 Name 으로 보내는게 맞을지 UID 로 보내는게 맞을지?
                // _action?.Invoke(NetworkManager.Singleton.LocalClientId, Name, type.ToString(), eventId);
            });
            return entry;
        }

        private EventTrigger.Entry CreateEntry(EventTriggerType type, List<Action<IElement>> eventActions)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = type
            };

            // _lastEventTimes[type] = 0;
            entry.callback.AddListener(_ =>
            {
                // var diff = Time.time - _lastEventTimes[type];
                // if (diff < Threshold)
                // {
                //     return;
                // }

                // _lastEventTimes[type] = Time.time;
                foreach (var eventAction in eventActions)
                {
                    eventAction?.Invoke(this);
                }
            });
            return entry;
        }

        public void AddPointerEnterListener(Action<IElement> eventAction)
        {
            var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerEnter);
            if (entry == null)
            {
                _eventTrigger.triggers.Add(CreateEntry(EventTriggerType.PointerEnter, _pointerEnterEvents));
            }

            _pointerEnterEvents.Add(eventAction);
        }

        public void RemovePointerEnterListener(Action<IElement> eventAction)
        {
            _pointerEnterEvents.Remove(eventAction);
        }

        public void RemoveAllPointerEnterListener(Action<IElement> eventAction)
        {
            _pointerEnterEvents.Clear();
        }

        public void AddPointerExitListener(Action<IElement> eventAction)
        {
            var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerExit);
            if (entry == null)
            {
                _eventTrigger.triggers.Add(CreateEntry(EventTriggerType.PointerExit, _pointerExitEvents));
            }

            _pointerExitEvents.Add(eventAction);
        }

        public void RemovePointerExitListener(Action<IElement> eventAction)
        {
            _pointerExitEvents.Remove(eventAction);
        }

        public void RemoveAllPointerExitListener(Action<IElement> eventAction)
        {
            _pointerExitEvents.Clear();
        }

        public void AddPointerDownListener(Action<IElement> eventAction)
        {
            var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerDown);
            if (entry == null)
            {
                _eventTrigger.triggers.Add(CreateEntry(EventTriggerType.PointerDown, _pointerDownEvents));
            }

            _pointerDownEvents.Add(eventAction);
        }

        public void RemovePointerDownListener(Action<IElement> eventAction)
        {
            _pointerDownEvents.Remove(eventAction);
        }

        public void RemoveAllPointerDownListener(Action<IElement> eventAction)
        {
            _pointerDownEvents.Clear();
        }

        public void AddPointerUpListener(Action<IElement> eventAction)
        {
            var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerUp);
            if (entry == null)
            {
                _eventTrigger.triggers.Add(CreateEntry(EventTriggerType.PointerUp, _pointerUpEvents));
            }

            _pointerUpEvents.Add(eventAction);
        }

        public void RemovePointerUpListener(Action<IElement> eventAction)
        {
            _pointerUpEvents.Remove(eventAction);
        }

        public void RemoveAllPointerUpListener(Action<IElement> eventAction)
        {
            _pointerUpEvents.Clear();
        }
    }
}