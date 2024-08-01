using System;
using System.Collections.Generic;

namespace V8
{
    [Obsolete]
    [Serializable]
    public class GridLayoutData : LayoutData
    {
        public List<int> padding;
        public List<float> childSize;
        public List<float> spacing;
        public string corner;
        public string axis;
        public string childAlignment;
        public string childConstraint;
        public int childConstraintCount;
    }
}