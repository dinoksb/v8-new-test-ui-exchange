using UnityEngine;
using G2.UI.Elements;

namespace G2.UI
{
    public class ImageComponents : FrameComponents
    {
        private const string _IMAGE_SOURCE_NAME = "SourceImage";
        public UnityEngine.UI.Image BackGroundImage { get; }
        public UnityEngine.UI.Image Image { get; }
        public new TransformLinkComponent TransformLinkComponent { get; }

        public ImageComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            RectTransform backgroundRectTransform = Self;
            // create image transform
            RectTransform imageRectTransform = CreateUIElement(_IMAGE_SOURCE_NAME, Self, true);
            
            if (zIndexParent)
            {
                // create background image source
                backgroundRectTransform = CreateUIElement(Self.name, zIndexParent, false);
                // create image source
                imageRectTransform = CreateUIElement(_IMAGE_SOURCE_NAME, backgroundRectTransform, true);
                
                // add link component
                TransformLinkComponent = backgroundRectTransform.gameObject.AddComponent<TransformLinkComponent>();
            }
            
            // add image and link component
            BackGroundImage = backgroundRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
            Image = imageRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
        }
    }
}
