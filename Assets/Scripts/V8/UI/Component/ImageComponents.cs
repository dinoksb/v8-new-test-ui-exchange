using UnityEngine;

namespace V8
{
    public class ImageComponents : FrameComponents
    {
        public UnityEngine.UI.Image BackGroundImage { get; }
        public UnityEngine.UI.Image Image { get; }

        public ImageComponents(IElement parent, string name) : base(parent, name)
        {
            var bgElement = Self;
            bgElement.SetParent(Self);
            bgElement.anchorMin = Vector2.zero;
            bgElement.anchorMax = Vector2.one;
            bgElement.offsetMin = Vector2.zero;
            bgElement.offsetMax = Vector2.zero;

            var imageGo = new GameObject("SourceImage");
            var imageElement = imageGo.AddComponent<RectTransform>();
            imageElement.SetParent(bgElement);
            imageElement.anchorMin = Vector2.zero;
            imageElement.anchorMax = Vector2.one;
            imageElement.offsetMin = Vector2.zero;
            imageElement.offsetMax = Vector2.zero;
            imageGo.layer = LayerMask.NameToLayer(UIConfig.LayerName);

            BackGroundImage = bgElement.gameObject.AddComponent<UnityEngine.UI.Image>();
            Image = imageElement.gameObject.AddComponent<UnityEngine.UI.Image>();
        }
    }
}