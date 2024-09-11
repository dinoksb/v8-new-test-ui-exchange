using UnityEngine;

namespace V8
{
    public class FrameComponents : ElementComponents
    {
        public UnityEngine.UI.Image Dim { get; }
        
        public FrameComponents(IElement parent, string name) : base(parent, name)
        {
        }
        
        public FrameComponents(IElement parent, string name, float dimOpacity, Vector2 referenceResolution) : base(parent, name)
        {
            if (dimOpacity != 0)
            {
                GameObject go = new GameObject("Dim");
                go.transform.SetParent(Self);
                go.layer = LayerMask.NameToLayer("UI");
                var rectTransform = go.AddComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(referenceResolution.x, referenceResolution.y);
            
                var image = go.AddComponent<UnityEngine.UI.Image>();
                image.color = new Color(0,0,0,dimOpacity);

                Dim = image;
            }
        }
    }
}