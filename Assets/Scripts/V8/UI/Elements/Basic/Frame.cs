using System;
using UnityEngine;
using ConstraintType = V8.FrameData.ConstraintType;

namespace V8
{
    public class Frame : Element
    {
        private bool _isOnUpdateSizeSubscribed;

        public ConstraintType ConstraintType;
        public bool Interactable;


        public Frame(FrameData data, FrameComponents components) : base(data, components)
        {
            ConstraintType = data.sizeConstraint;
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Frame)base.Copy(self, parent);

            if (clone._isOnUpdateSizeSubscribed)
            {
                clone.Parent.Dispose();
                clone.Parent.OnUpdateSize += clone.UpdateSize;
            }

            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var layoutData = (FrameData)data;
            SetValues(layoutData);
        }

        private void UpdateSize(object _, Vector2 size)
        {
            Size = size;
        }

        private void SetValues(FrameData data)
        {
            Interactable = data.interactable;
            Size = CalculateSize(data.size, true);
            Position = CalculatePosition(data.position, true);
            Rotation = data.rotation;
        }

        private Vector2 CalculatePosition(DimensionData data, bool relative)
        {
            return CalculateDimension(data, relative);
        }

        private Vector2 CalculateSize(DimensionData data, bool relative)
        {
            if (Parent == this)
            {
                _isOnUpdateSizeSubscribed = true;
                Parent.OnUpdateSize += UpdateSize;
                return Parent.Size;
            }

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
    }
}