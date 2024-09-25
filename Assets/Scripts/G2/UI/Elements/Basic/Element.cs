using System;
using System.Collections.Generic;
using G2.Model.UI;
using G2.UI.Component;
using UnityEngine;
using Utilities;

namespace G2.UI.Elements.Basic
{
    public abstract class Element : IElement
    {
        public string Uid { get; }
        public string Name { get; }
        public string Type { get; }
        public uint ZIndex { get; }

        public IElement Parent { get; private set; }
        
        public RectTransform Self { get; private set; }
        
        public RectTransform ParentRectTransform { get; private set; }

        public Vector2 Anchor
        {
            get => Self.anchorMin;
            set
            {
                Self.anchorMin = value;
                Self.anchorMax = value;
            }
        }

        public Vector2 Pivot
        {
            get => Self.pivot;
            set => Self.pivot = value;
        }

        public Vector2 Size
        {
            get => Self.sizeDelta;
            set
            {
                foreach (var eventAction in _sizeChangeActions)
                {
                    eventAction?.Invoke(this);
                }

                Self.sizeDelta = value;
                _onSizeUpdatedCore?.Invoke(this, value);
            }
        }

        public Vector2 Position
        {
            get => Self.anchoredPosition;
            set
            {
                foreach (var eventAction in _positionChangeActions)
                {
                    eventAction?.Invoke(this);
                }
                Self.anchoredPosition = value;
                _onPositionUpdatedCore?.Invoke(this, Self.anchoredPosition);
            }
        }

        public float Rotation
        {
            get => Self.eulerAngles.z;
            set
            {
                foreach (var eventAction in _rotationChangeActions)
                {
                    eventAction?.Invoke(this);
                }

                float deltaZ = value - Self.eulerAngles.z;
                Self.Rotate(0, 0, deltaZ);
                _onRotationUpdatedCore?.Invoke(this, deltaZ);
            }
        }

        public virtual bool Visible
        {
            get => Self.gameObject.activeSelf;
            set
            {
                Self.gameObject.SetActive(value);
                foreach (var eventAction in _visibleChangedActions)
                {
                    eventAction?.Invoke(this);
                }
            }
        }

        event EventHandler<Vector2> IElement.OnSizeUpdated
        {
            add => _onSizeUpdatedCore += value;
            remove => _onSizeUpdatedCore -= value;
        }

        event EventHandler<Vector2> IElement.OnPositionUpdated
        {
            add => _onPositionUpdatedCore += value;
            remove => _onPositionUpdatedCore -= value;
        }

        event EventHandler<float> IElement.OnRotationUpdated
        {
            add => _onRotationUpdatedCore += value;
            remove => _onRotationUpdatedCore -= value;
        }

        private event EventHandler<Vector2> _onSizeUpdatedCore;
        private event EventHandler<Vector2> _onPositionUpdatedCore; 
        private event EventHandler<float> _onRotationUpdatedCore; 

        protected readonly List<Action<IElement>> _visibleChangedActions = new();
        protected readonly List<Action<IElement>> _positionChangeActions = new();
        protected readonly List<Action<IElement>> _rotationChangeActions = new();
        protected readonly List<Action<IElement>> _sizeChangeActions = new();

        protected Element(string uid, ElementData data, ElementComponents components)
        {
            Uid = uid;
            Name = data.Name;
            Type = data.Type;
            ZIndex = data.ZIndex;
            Self = components.Self;
            Parent = components.Parent;
            ParentRectTransform = components.ParentRectTransform;
            SetValues(data);
        }

        public void Dispose()
        {
            _onSizeUpdatedCore = null;
            _onPositionUpdatedCore = null;
            _onRotationUpdatedCore = null;
        }

        public virtual IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement)
        {
            var clone = (Element)MemberwiseClone();
            clone.Self = self;
            clone.Parent = parentElement;
            clone.ParentRectTransform = parentRectTransform;

            return clone;
        }


        public virtual void Update(ElementData data)
        {
            SetValues(data);
            Visible = data.Visible;
        }

        private void SetValues(ElementData data)
        {
            Anchor = TypeConverter.ToVector2(data.Anchor).ToReverseYAxis();
            Pivot = TypeConverter.ToVector2(data.Pivot).ToReverseYAxis();
        }

        # region internal events
        void IElement.AddVisibleChangedListener(Action<IElement> action)
        {
            _visibleChangedActions.Add(action);
        }

        void IElement.RemoveVisibleChangedListener(Action<IElement> action)
        {
            _visibleChangedActions.Remove(action);
        }

        void IElement.RemoveAllVisibleChangedListener()
        {
            _visibleChangedActions.Clear();
        }

        void IElement.AddPositionChangeListener(Action<IElement> action)
        {
            _positionChangeActions.Add(action);
        }

        void IElement.RemovePositionChangeListener(Action<IElement> action)
        {
            _positionChangeActions.Remove(action);
        }

        void IElement.RemoveAllPositionChangeListener()
        {
            _positionChangeActions.Clear();
        }

        void IElement.AddRotationChangeListener(Action<IElement> action)
        {
            _rotationChangeActions.Add(action);
        }

        void IElement.RemoveRotationChangeListener(Action<IElement> action)
        {
            _rotationChangeActions.Remove(action);
        }

        void IElement.RemoveAllRotationChangeListener()
        {
            _rotationChangeActions.Clear();
        }

        void IElement.AddSizeChangeListener(Action<IElement> action)
        {
            _sizeChangeActions.Add(action);
        }

        void IElement.RemoveSizeChangeListener(Action<IElement> action)
        {
            _sizeChangeActions.Remove(action);
        }

        void IElement.RemoveAllSizeChangeListener()
        {
            _sizeChangeActions.Clear();
        }

        #endregion
    }
}
