using System;
using UnityEngine;
using UnityEngine.UI;

namespace V8
{
    // Note: 현 시점에서는 필요없어 보임
    [Obsolete]
    public class GridLayout : Layout
    {
        private GridLayoutGroup _gridlayout;

        public GridLayoutGroup.Corner Corner
        {
            get => _gridlayout.startCorner;
            set => _gridlayout.startCorner = value;
        }

        public GridLayoutGroup.Axis Axis
        {
            get => _gridlayout.startAxis;
            set => _gridlayout.startAxis = value;
        }

        public TextAnchor ChildAlignment
        {
            get => _gridlayout.childAlignment;
            set => _gridlayout.childAlignment = value;
        }

        public GridLayoutGroup.Constraint ChildConstraint
        {
            get => _gridlayout.constraint;
            set => _gridlayout.constraint = value;
        }

        public RectOffset Padding
        {
            get => _gridlayout.padding;
            set => _gridlayout.padding = value;
        }

        public Vector2 ChildSize
        {
            get => _gridlayout.cellSize;
            set => _gridlayout.cellSize = value;
        }

        public Vector2 Spacing
        {
            get => _gridlayout.spacing;
            set => _gridlayout.spacing = value;
        }

        public int ChildConstraintCount
        {
            get => _gridlayout.constraintCount;
            set => _gridlayout.constraintCount = value;
        }

        public GridLayout(GridLayoutData data, GridLayoutComponents components) : base(data, components)
        {
            _gridlayout = components.GridLayout;
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (GridLayout)base.Copy(self, parent);
            clone._gridlayout = self.GetComponent<GridLayoutGroup>();
            return clone;
        }
        
        public override void Update(ElementData data)
        {
            base.Update(data);
            var gridLayoutData = (GridLayoutData)data;
            SetValues(gridLayoutData);
        }

        private void SetValues(GridLayoutData data)
        {
            Corner = TypeConverter.ToCorner(data.corner);
            Axis = TypeConverter.ToAxis(data.axis);
            ChildAlignment = TypeConverter.ToTextAnchor(data.childAlignment);
            ChildConstraint = TypeConverter.ToConstraint(data.childConstraint);
            Padding = TypeConverter.ToRectOffset(data.padding);
            ChildSize = TypeConverter.ToVector2(data.childSize);
            Spacing = TypeConverter.ToVector2(data.spacing);
            ChildConstraintCount = data.childConstraintCount;
        }
    }
}