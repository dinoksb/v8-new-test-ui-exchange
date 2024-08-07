using UnityEngine;

namespace V8
{
    public class ImageComponents : FrameComponents
    {
        public UnityEngine.UI.Image Image { get; }

        public ImageComponents(IElement parent, string id) : base(parent, id)
        {
            var go = new GameObject(UIConfig.Element);
            var element = go.AddComponent<RectTransform>();
            element.SetParent(Self);
            element.anchorMin = Vector2.zero;
            element.anchorMax = Vector2.one;
            element.offsetMin = Vector2.zero;
            element.offsetMax = Vector2.zero;

            Image = go.gameObject.AddComponent<UnityEngine.UI.Image>();
        }
    }
}