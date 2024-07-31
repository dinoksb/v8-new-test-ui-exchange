using UnityEngine;

namespace V8
{
    public class ElementComponents
    {
        public IElement Parent { get; }
        public RectTransform Self { get; }

        public ElementComponents(IElement parent, string id)
        {
            var go = new GameObject(id);
            var self = go.AddComponent<RectTransform>();
            self.SetParent(parent.Self);
            self.localPosition = Vector3.zero;
            self.localRotation = Quaternion.identity;
            self.localScale = Vector3.one;
            
            Parent = parent;
            Self = self;
        }
    }
}