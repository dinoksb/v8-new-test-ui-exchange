using System;
using G2.Model.UI;
using G2.UI.Component;
using UnityEngine;

namespace G2.UI.Elements.Basic
{
    public abstract class UpdatableElement : Element
    {
        public UpdatableElementData.ConstraintType constraintType;

        public virtual bool Interactable { get; set; }

        private Vector2 _sizeRatio;
        private Vector2 _sizeOffset;
        private bool _isOnUpdateSizeSubscribed;

        protected UpdatableElement(string uid, UpdatableElementData data, UpdatableElementComponents components) : base(uid, data, components)
        {
            var elementTransformLink = components.ElementTransformLinkComponent;
            elementTransformLink.Initialize(Self);
            var visualTransformLink = components.VisualTransformLinkComponent;
            visualTransformLink.Initialize(elementTransformLink.Self);
            
            constraintType = data.sizeConstraint;
            SetTransformLink(elementTransformLink);
            SetEvents();
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement)
        {
            var clone = (UpdatableElement)base.Copy(self, parentRectTransform, parentElement);

            if (clone._isOnUpdateSizeSubscribed)
            {
                clone.Parent.Dispose();
                clone.Parent.OnSizeUpdated += clone.SizeUpdated;
            }

            clone.Parent.AddVisibleChangedListener(clone.VisibleChanged);
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var layoutData = (FrameData)data;
            SetValues(layoutData);
        }

        private void SizeUpdated(object _, Vector2 size)
        {
            var x = _sizeRatio.x == 0 ? Size.x : (size.x * _sizeRatio.x) + _sizeOffset.x;
            var y = _sizeRatio.y == 0 ? Size.y : (size.y * _sizeRatio.y) + _sizeOffset.y;
            var calcByRatio = new Vector2(x, y);
            Debug.Log($"[{_}] Parent size updated: {calcByRatio}");
            Size = calcByRatio;
        }

        protected void VisibleChanged(IElement element)
        {
            Debug.Log($"VisibleChanged from {element.Name} to {Name}: {element.Visible}");
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

        private void SetValues(ElementData data)
        {
            Size = CalculateDimension(data.Size, true);
            Position = CalculateDimension(data.Position, true);
            Rotation = data.Rotation;
            _sizeRatio = new Vector2(data.Size.X.Scale, data.Size.Y.Scale);
            _sizeOffset = new Vector2(data.Size.X.Offset, data.Size.Y.Offset);
        }

        private void SetEvents()
        {
            if (Parent == null) return;

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
        }

        private Vector2 CalculateDimension(DimensionData data, bool relative)
        {
            // Decide whether to use the width and height of the parent Transform or the width and height of the Screen.
            var relativeWidth = relative ? ParentRectTransform.rect.width : Screen.width;
            var relativeHeight = relative ? ParentRectTransform.rect.height : Screen.height;

            float x;
            float y;

            switch (constraintType)
            {
                case UpdatableElementData.ConstraintType.XY:
                    // Calculate the actual value to be applied using the offset and scale values (considering both width and height).
                    x = data.X.Offset + data.X.Scale * relativeWidth;
                    y = data.Y.Offset + data.Y.Scale * relativeHeight;
                    break;

                case UpdatableElementData.ConstraintType.XX:
                    // Calculate considering only the horizontal resolution.
                    x = data.X.Offset + data.X.Scale * relativeWidth;
                    y = data.Y.Offset + data.Y.Scale * relativeWidth; // Apply the horizontal resolution to the vertical as well.
                    break;

                case UpdatableElementData.ConstraintType.YY:
                    // Calculate considering only the vertical resolution.
                    x = data.X.Offset + data.X.Scale * relativeHeight; // Apply the vertical resolution to the horizontal as well.
                    y = data.Y.Offset + data.Y.Scale * relativeHeight;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(constraintType), $"Unhandled ConstraintType enum value: {constraintType}");
            }

            return new Vector2(x, y);
        }
    }
}
