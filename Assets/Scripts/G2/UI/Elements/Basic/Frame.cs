using System;
using UnityEngine;
using G2.Model.UI;
using Utilities;
using ConstraintType = G2.Model.UI.FrameData.ConstraintType;

namespace G2.UI.Elements.Basic
{
    public class Frame : Element
    {
        private const string _DIM_TYPE = "Dim";
        public override bool Visible
        {
            get => Self.gameObject.activeSelf;
            set
            {
                Self.gameObject.SetActive(value);
                if (_dim) _dim.gameObject.SetActive(value);
                foreach (var eventAction in _visibleChangedActions)
                {
                    eventAction?.Invoke(this);
                }
            }
        }

        public ConstraintType ConstraintType;

        public virtual bool Interactable { get; set; }

        private UnityEngine.UI.Image _dim;
        private TransformLinkComponent _transformLink;
        
        private Vector2 _sizeRatio;
        private Vector2 _sizeOffset;
        private bool _isOnUpdateSizeSubscribed;

        public Frame(string uid, FrameData data, FrameComponents components) : base(uid, data, components)
        {
            ConstraintType = data.sizeConstraint;
            SetValues(data);
            SetEvents(data);
        }

        public Frame(string uid, FrameData data, FrameComponents components, Transform zIndexParent) : base(uid, data,
            components)
        {
            if (zIndexParent)
            {
                _transformLink = components.TransformLinkComponent;
                SetTransformLink(_transformLink);
                if (data.dim != 0)
                {
                    _dim = CreateDim(zIndexParent, data.dim, Vector3.zero);
                    _dim.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    _dim.rectTransform.anchorMin = Vector2.zero;
                    _dim.rectTransform.anchorMax = Vector2.one;
                }
            }
            else
            {
                _dim = CreateDim(Self, data.dim, new Vector2(Screen.width, Screen.height));
            }

            ConstraintType = data.sizeConstraint;
            SetValues(data);
            SetEvents(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Frame)base.Copy(self, parent);

            if (clone._isOnUpdateSizeSubscribed)
            {
                clone.Parent.Dispose();
                clone.Parent.OnSizeUpdated += clone.SizeUpdated;
            }

            clone.Parent.AddVisibleChangedListener(clone.VisibleChanged);
            clone.Parent.OnMoveFront += clone.MoveFront;
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var layoutData = (FrameData)data;
            SetValues(layoutData);
        }

        public override void MoveFront()
        {
            if (_dim)
            {
                _dim.transform.SetAsLastSibling();
            }
            base.MoveFront();
        }

        private void SizeUpdated(object _, Vector2 size)
        {
            var x = _sizeRatio.x == 0 ? Size.x : (size.x * _sizeRatio.x) + _sizeOffset.x;
            var y = _sizeRatio.y == 0 ? Size.y : (size.y * _sizeRatio.y) + _sizeOffset.y;
            var calcByRatio = new Vector2(x, y);
            InternalDebug.Log($"[{_}] Parent size updated: {calcByRatio}");
            Size = calcByRatio;
        }

        protected void VisibleChanged(IElement element)
        {
            InternalDebug.Log($"VisibleChanged from {element.Name} to {Name}: {element.Visible}");
            Visible = element.Visible;
        }

        protected void PositionChanged(object _, Vector2 position)
        {
            Position = Position;
        }
        
        protected void RotationChanged(object _, float rotationY)
        {
            Rotation = Rotation;
        }
        
        protected void SetTransformLink(TransformLinkComponent linkComponent)
        {
            if (!linkComponent) return;
            linkComponent.Initialize(Self);
            
            _visibleChangedActions.Add(linkComponent.SetVisible);
            _positionChangeActions.Add(linkComponent.SetPosition);
            _rotationChangeActions.Add(linkComponent.SetRotation);
            _sizeChangeActions.Add(linkComponent.SetSize);
        }

        private void SetValues(FrameData data)
        {
            Size = CalculateSize(data.size, true);
            Position = CalculatePosition(data.position, true);
            Rotation = data.rotation;
            _sizeRatio = new Vector2(data.size.x.scale, data.size.y.scale);
            _sizeOffset = new Vector2(data.size.x.offset, data.size.y.offset);
        }

        private void SetEvents(FrameData data)
        {
            if (CheckIsCanvas(Parent)) return;
            
            // need to follow the size of the parent element
            if (_sizeRatio != Vector2.zero)
            {
                _isOnUpdateSizeSubscribed = true;
                Parent.OnSizeUpdated += SizeUpdated;
            }

            // parent element's visibility is turned off, the child element is also hidden.
            Parent.AddVisibleChangedListener(VisibleChanged);
            Parent.OnPositionUpdated += PositionChanged;
            Parent.OnRotationUpdated += RotationChanged;
            Parent.OnMoveFront += MoveFront;
        }

        private Vector2 CalculatePosition(DimensionData data, bool relative)
        {
            return CalculateDimension(data, relative);
        }

        private Vector2 CalculateSize(DimensionData data, bool relative)
        {
            // // todo: 부모 객체의 사이즈 변경 시 어떻게 처리 할 것인지??
            // if (Parent == this)
            // {
            //     _isOnUpdateSizeSubscribed = true;
            //     Parent.OnUpdateSize += UpdateSize;
            //     return Parent.Size;
            // }

            return CalculateDimension(data, relative);
        }

        private Vector2 CalculateDimension(DimensionData data, bool relative)
        {
            // 부모 Transform 의 width, height 를 사용할 것인지 Screen 의 width, height 를 사용할 것인지 결정.
            var relativeWidth = relative ? Parent.Self.rect.width : Screen.width;
            var relativeHeight = relative ? Parent.Self.rect.height : Screen.height;

            float x = 0;
            float y = 0;

            switch (ConstraintType)
            {
                case ConstraintType.XY:
                    // offset 값과 scale 값을 이용해 실제 적용하게 될 값 계산 (가로와 세로 모두 고려)
                    x = data.x.offset + data.x.scale * relativeWidth;
                    y = data.y.offset + data.y.scale * relativeHeight;
                    break;

                case ConstraintType.XX:
                    // 가로 해상도만 고려하여 계산
                    x = data.x.offset + data.x.scale * relativeWidth;
                    y = data.y.offset + data.y.scale * relativeWidth; // 가로 해상도를 세로에도 적용
                    break;

                case ConstraintType.YY:
                    // 세로 해상도만 고려하여 계산
                    x = data.x.offset + data.x.scale * relativeHeight; // 세로 해상도를 가로에도 적용
                    y = data.y.offset + data.y.scale * relativeHeight;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ConstraintType),
                        $"처리할 수 없는 ConstraintType enum 값입니다: {ConstraintType}");
            }

            // offset 값과 scale 값을 이용해 실제 적용하게 될 값 계산
            // var x = data.x.offset + data.x.scale * relativeWidth;
            // var y = data.y.offset + data.y.scale * relativeHeight;

            return new Vector2(x, y);
        }

        private UnityEngine.UI.Image CreateDim(Transform parent, float opacity, Vector2 size)
        {
            GameObject dimObj = new GameObject(_DIM_TYPE);
            dimObj.transform.SetParent(parent);

            var rectTransform = dimObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var image = dimObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, opacity);
            return image;
        }
    }
}
