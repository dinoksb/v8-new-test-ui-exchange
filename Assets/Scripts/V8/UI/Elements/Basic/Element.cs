using System;
using System.Collections.Generic;
using UnityEngine;
using V8.Service;

namespace V8
{
    public class Element : IElement
    {
        public string Uid { get; }
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
                    eventAction?.Invoke(this);
                }
            }
        }

        public IElement Parent { get; private set; }

        public event EventHandler<Vector2> OnUpdateSize;

        private IUIService _uiService;
        
        private readonly List<Action<IElement>> _visibleChangedActions = new();
        private readonly List<Action<IElement>> _positionChangeActions = new();
        private readonly List<Action<IElement>> _rotationChangeActions = new();
        private readonly List<Action<IElement>> _sizeChangeActions = new();

        public Element(string uid, ElementData data, ElementComponents components)
        {
            // todo: 추후 DI 로 주입 해야함.
            _uiService = GameObject.FindObjectOfType<UIService>();
            
            _uiService.OnCreated(this);
            Uid = uid;
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
        
        # region internal events
        void IElement.AddVisibleChangedListener(Action<IElement> action)
        {
            if (!_visibleChangedActions.Contains(action))
            {
                _visibleChangedActions.Add(action);
            }
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
            if (!_positionChangeActions.Contains(action))
            {
                _positionChangeActions.Add(action);
            }
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
            if (!_rotationChangeActions.Contains(action))
            {
                _rotationChangeActions.Add(action);
            }
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
            if (!_sizeChangeActions.Contains(action))
            {
                _sizeChangeActions.Add(action);
            }
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