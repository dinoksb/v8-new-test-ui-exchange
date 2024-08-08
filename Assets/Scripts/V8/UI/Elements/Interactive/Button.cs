using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace V8
{
    public class Button : Frame
    {
        private EventTrigger _eventTrigger;
        private ReadOnlyDictionary<EventTriggerType, string> _events;
        private readonly Action<ulong, string, string, string> _action;
        private readonly Dictionary<EventTriggerType, float> _lastEventTimes = new();

        public ReadOnlyDictionary<EventTriggerType, string> Events
        {
            get => _events;
            private set
            {
                _events = value;
                UpdateEventTrigger(_events);
            }
        }

        public float Threshold { get; }

        public Button(ButtonData data, ButtonComponents components, Action<ulong, string, string, string> action)
            : base(data, components)
        {
            _action = action;
            _eventTrigger = components.EventTrigger;
            Threshold = data.threshold;

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
            return clone;
        }

        private void UpdateEventTrigger(ReadOnlyDictionary<EventTriggerType, string> events)
        {
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

            _lastEventTimes[type] = 0;
            entry.callback.AddListener(_ =>
            {
                var diff = Time.time - _lastEventTimes[type];
                if (diff < Threshold)
                {
                    return;
                }

                _lastEventTimes[type] = Time.time;
                //Todo: Network 로 이벤트 보낼 때 Name 으로 보내는게 맞을지 UID 로 보내는게 맞을지?
                _action.Invoke(NetworkManager.Singleton.LocalClientId, Name, type.ToString(), eventId);
            });
            return entry;
        }
    }
}