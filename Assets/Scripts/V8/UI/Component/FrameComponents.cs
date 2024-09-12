using UnityEngine;

namespace V8
{
    public class FrameComponents : ElementComponents
    {
        public UnityEngine.UI.Image Dim { get; }

        public FrameComponents(IElement parent, string name) : base(parent, name)
        {
        }

        public FrameComponents(IElement parent, string name, bool isUseDim) : base(parent, name)
        {
            if (!isUseDim) return;
            
            GameObject go = new GameObject(UIConfig.DimType);
            go.transform.SetParent(Self);
            go.layer = LayerMask.NameToLayer(UIConfig.LayerName);
            
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, 0);

            Dim = image;
        }
    }
}