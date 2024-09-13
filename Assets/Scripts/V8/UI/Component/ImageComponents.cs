using UnityEngine;

namespace V8
{
    public class ImageComponents : FrameComponents
    {
        public UnityEngine.UI.Image BackGroundImage { get; }
        public UnityEngine.UI.Image Image { get; }
        public TransformLinkComponents TransformLinkComponents { get; }

        public ImageComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            // set background transform
            var bgRectTransform = Self;
            bgRectTransform.SetParent(Self);
            
            // create image transform
            var imageGo = new GameObject(UIConfig.ImageSource);
            var imageRectTransform = imageGo.AddComponent<RectTransform>();
            imageRectTransform.SetParent(bgRectTransform);
            imageRectTransform.anchorMin = Vector2.zero;
            imageRectTransform.anchorMax = Vector2.one;
            imageRectTransform.offsetMin = Vector2.zero;
            imageRectTransform.offsetMax = Vector2.zero;
            
            // create background image source
            var bgImageSource = new GameObject(Self.name);
            var bgSourceRectTransform = bgImageSource.AddComponent<RectTransform>();
            bgSourceRectTransform.SetParent(zIndexParent);
            bgSourceRectTransform.localPosition = Vector3.zero;
            bgSourceRectTransform.localRotation = Quaternion.identity;
            bgSourceRectTransform.localScale = Vector3.one;
            
            // create image source
            var imageSourceGo = new GameObject(UIConfig.ImageSource);
            var imageSourceRectTransform = imageSourceGo.AddComponent<RectTransform>();
            imageSourceRectTransform.SetParent(bgSourceRectTransform);
            imageSourceRectTransform.anchorMin = Vector2.zero;
            imageSourceRectTransform.anchorMax = Vector2.one;
            imageSourceRectTransform.offsetMin = Vector2.zero;
            imageSourceRectTransform.offsetMax = Vector2.zero;
            imageSourceGo.layer = LayerMask.NameToLayer(UIConfig.LayerName);
            
            // add image and link component
            TransformLinkComponents = bgImageSource.AddComponent<TransformLinkComponents>().Attach(bgRectTransform);
            BackGroundImage = bgImageSource.AddComponent<UnityEngine.UI.Image>();
            Image = imageSourceRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
        }
    }
}