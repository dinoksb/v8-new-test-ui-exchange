using System;
using System.Collections.Generic;

namespace V8
{
    [Serializable]
    public class ElementData
    {
        public string id;
        public string type;
        public bool visible;
        public List<float> anchorMin;
        public List<float> anchorMax;
        public List<float> pivot;
        public List<string> size;
        public string positionLayout;
        public List<string> position;
        public List<ElementData> children;

        public ElementData GetChildById(string elementId)
        {
            return GetChild(child => child.id.Equals(elementId));
        }

        public ElementData GetChildByType(string elementType)
        {
            return GetChild(child => child.type.Equals(elementType));
        }

        public List<ElementData> GetChildrenByType(string elementType)
        {
            var matchedElements = new List<ElementData>();
            if (type.Equals(elementType))
            {
                matchedElements.Add(this);
            }

            foreach (var child in children)
            {
                matchedElements.AddRange(child.GetChildrenByType(elementType));
            }

            return matchedElements;
        }

        private ElementData GetChild(Func<ElementData, bool> condition)
        {
            if (condition(this)) return this;

            foreach (var child in children)
            {
                var childData = child.GetChild(condition);
                if (childData != null) return childData;
            }

            return default;
        }
    }
}