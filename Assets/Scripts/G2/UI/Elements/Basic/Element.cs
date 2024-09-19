using System;
using System.Collections.Generic;
using UnityEngine;
using G2.Model.UI;
using G2.Service;
using Utilities;

namespace G2.UI.Elements.Basic
{
    public class Element : IElement
    {
        public string Uid { get; }
        public string Name { get; }
        public string Type { get; }

        public RectTransform Self { get; private set; }

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

        event Action IElement.OnMoveFront
        {
            add => _onMoveFrontCore += value;
            remove => _onMoveFrontCore -= value;
        }

        private event EventHandler<Vector2> _onSizeUpdatedCore;
        private event EventHandler<Vector2> _onPositionUpdatedCore; 
        private event EventHandler<float> _onRotationUpdatedCore; 
        private event Action _onMoveFrontCore; 
        
        public IElement Parent { get; private set; }
        
        public uint ZIndex { get; }

        private IUIService _uiService;

        protected readonly List<Action<IElement>> _visibleChangedActions = new();
        protected readonly List<Action<IElement>> _positionChangeActions = new();
        protected readonly List<Action<IElement>> _rotationChangeActions = new();
        protected readonly List<Action<IElement>> _sizeChangeActions = new();

        public Element(string uid, ElementData data, ElementComponents components)
        {
            // todo: 추후 DI 로 주입 해야함.
            _uiService = GameObject.FindObjectOfType<UIService>();
            _uiService.OnCreated(this);
            
            Uid = uid;
            Name = data.name;
            Type = data.type;
            ZIndex = data.zIndex;
            Self = components.Self;
            Parent = components.Parent;
            SetValues(data);
        }

        public void Dispose()
        {
            _onSizeUpdatedCore = null;
            _onPositionUpdatedCore = null;
            _onRotationUpdatedCore = null;
            _onMoveFrontCore = null;
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

        public virtual void MoveFront()
        {
            _onMoveFrontCore?.Invoke();
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

        protected bool CheckIsCanvas(IElement element)
        {
            return element.Type == nameof(Canvas);
        }

        private void SetValues(ElementData data)
        {
            Anchor = TypeConverter.ToVector2(data.anchor).ToReverseYAxis();
            Pivot = TypeConverter.ToVector2(data.pivot).ToReverseYAxis();
        }

        # region internal events

        void IElement.MoveFront()
        {
            MoveFront();
        }

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
