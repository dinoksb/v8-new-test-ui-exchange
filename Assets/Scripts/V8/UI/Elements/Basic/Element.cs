using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    public class Element : IElement
    {
        public string Name { get; }

        public string Type { get; }

        public RectTransform Self { get; private set; }

        public Vector2 AnchorMin
        {
            get => Self.anchorMin;
            set => Self.anchorMin = value;
        }

        public Vector2 AnchorMax
        {
            get => Self.anchorMax;
            set => Self.anchorMax = value;
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
                OnUpdateSize?.Invoke(this, value);
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
            }
        }

        public bool Visible
        {
            get => Self.gameObject.activeSelf;
            set
            {
                Self.gameObject.SetActive(value);
                foreach (var eventAction in _visibleChangedActions)
                {
                    eventAction?.Invoke(value);
                }
            }
        }

        public IElement Parent { get; private set; }

        public event EventHandler<Vector2> OnUpdateSize;

        private readonly List<Action<bool>> _visibleChangedActions;
        private readonly List<Action<IElement>> _positionChangeActions;
        private readonly List<Action<IElement>> _rotationChangeActions;
        private readonly List<Action<IElement>> _sizeChangeActions;

        public Element(ElementData data, ElementComponents components)
        {
            Name = data.name;
            Type = data.type;
            Self = components.Self;
            Parent = components.Parent;
            SetValues(data);
        }

        public void Dispose()
        {
            OnUpdateSize = null;
        }

        public virtual IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Element)MemberwiseClone();
            clone.Self = self;
            clone.Parent = parent;
            
            return clone;
        }


        public virtual void Update(ElementData data)
        {
            SetValues(data);
            
            Visible = data.visible;
        }
        
        protected static RectTransform GetChildSelf(Transform targetSelf, string id)
        {
            for (var i = 0; i < targetSelf.childCount; i++)
            {
                var newChild = targetSelf.GetChild(i);
                if (newChild.name == id)
                {
                    return newChild.GetComponent<RectTransform>();
                }
            }

            throw new Exception($"No child with ID '{id}' found under {targetSelf.name}.");
        }

        private void SetValues(ElementData data)
        {
            AnchorMin = SetRectTransformAnchorPivot(data.anchorMin);
            AnchorMax = SetRectTransformAnchorPivot(data.anchorMax);
            Pivot = SetRectTransformAnchorPivot(data.pivot);

            Vector2 SetRectTransformAnchorPivot(IReadOnlyList<float> values)
            {
                var convertedValue = TypeConverter.ToVector2(values);
                return convertedValue == Vector2.zero ? new Vector2(0.5f, 0.5f) : convertedValue;
            }
        }
        # region Events
        void IElement.AddVisibleChanged(Action<bool> action)
        {
            if (!_visibleChangedActions.Contains(action))
            {
                _visibleChangedActions.Add(action);
            }
        }

        void IElement.RemoveVisibleChanged(Action<bool> action)
        {
            _visibleChangedActions.Remove(action);
        }

        void IElement.AddPositionChange(Action<IElement> action)
        {
            if (!_positionChangeActions.Contains(action))
            {
                _positionChangeActions.Add(action);
            }
        }

        void IElement.RemovePositionChange(Action<IElement> action)
        {
            _positionChangeActions.Remove(action);
        }

        void IElement.AddRotationChange(Action<IElement> action)
        {
            if (!_rotationChangeActions.Contains(action))
            {
                _rotationChangeActions.Add(action);
            }
        }

        void IElement.RemoveRotationChange(Action<IElement> action)
        {
            _rotationChangeActions.Remove(action);
        }

        void IElement.AddSizeChange(Action<IElement> action)
        {
            if (!_sizeChangeActions.Contains(action))
            {
                _sizeChangeActions.Add(action);
            }
        }

        void IElement.RemoveSizeChange(Action<IElement> action)
        {
            _sizeChangeActions.Remove(action);
        }
        #endregion
    }
}