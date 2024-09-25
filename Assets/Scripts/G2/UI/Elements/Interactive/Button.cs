using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using G2.Model.UI;
using G2.UI.Component;
using G2.UI.Elements.Basic;
using Utilities;

namespace G2.UI.Elements.Interactive
{
    public class Button : UpdatableElement
    {
        private readonly Action<ulong, string, string, string> _action;
        private readonly List<Action<IElement>> _pointerEnterEvents = new();
        private readonly List<Action<IElement>> _pointerExitEvents = new();
        private readonly List<Action<IElement>> _pointerDownEvents = new();
        private readonly List<Action<IElement>> _pointerUpEvents = new();
        private readonly List<Action<IElement>> _pointerDragEvents = new();
        private readonly NonDrawingGraphic _nonDrawingGraphic;
        
        private EventTrigger _eventTrigger;

        // private readonly Dictionary<EventTriggerType, float> _lastEventTimes = new();
        private TransformLinkComponent _elementTransformLink;

        public sealed override bool Interactable
        {
            get => _nonDrawingGraphic.raycastTarget;
            set => _nonDrawingGraphic.raycastTarget = value;
        }

        public Button(string uid, ButtonData data, ButtonComponents components,
            Action<ulong, string, string, string> action)
            : base(uid, data, components)
        {
            _action = action;
            _eventTrigger = components.EventTrigger;
            _nonDrawingGraphic = components.NonDrawingGraphic;
            Interactable = data.interactable;
            _elementTransformLink = components.ElementTransformLinkComponent;
            SetTransformLink(_elementTransformLink);
            SetEvents(data);
        }

        public override IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement)
        {
            var clone = (Button)base.Copy(self, parentRectTransform, parentElement);
            clone._eventTrigger = self.GetComponent<EventTrigger>();
            if (_elementTransformLink)
            {
                clone._elementTransformLink = _elementTransformLink;
                clone.SetTransformLink(clone._elementTransformLink);
            }

            clone.Parent.AddVisibleChangedListener(clone._elementTransformLink.SetVisible);

            return clone;
        }

        private void SetEvents(ButtonData data)
        {
            var e = new Dictionary<EventTriggerType, string>();
            foreach (var (eventType, eventId) in data.Events)
            {
                if (TypeConverter.TryEventTriggerType(eventType, out var type))
                {
                    e.Add(type, eventId);
                }
            }

            if (Parent == null) return;
            
            Parent.OnPositionUpdated += PositionChanged;
            Parent.AddVisibleChangedListener(VisibleChanged);
        }


        private EventTrigger.Entry CreateEntry(EventTriggerType type, List<Action<IElement>> eventActions)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = type
            };

            entry.callback.AddListener(_ =>
            {
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
        
        public void AddPointerDragListener(Action<IElement> eventAction)
        {
            var entry = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.Drag);
            if (entry == null)
            {
                _eventTrigger.triggers.Add(CreateEntry(EventTriggerType.Drag, _pointerDragEvents));
            }

            _pointerDragEvents.Add(eventAction);
        }

        public void RemovePointerDragListener(Action<IElement> eventAction)
        {
            _pointerDragEvents.Remove(eventAction);
        }

        public void RemoveAllPointerDragListener(Action<IElement> eventAction)
        {
            _pointerDragEvents.Clear();
        }
    }
}
