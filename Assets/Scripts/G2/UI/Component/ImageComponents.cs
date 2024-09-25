using G2.UI.Elements;
using UnityEngine;

namespace G2.UI.Component
{
    public class ImageComponents : UpdatableElementComponents
    {
        public UnityEngine.UI.Image BackGroundImage { get; }
        public UnityEngine.UI.Image Image { get; }

        private const string _SOURCE_IMAGE_NAME = "SourceImage";

        public ImageComponents(IElement parent, Transform parentTransform, Transform zIndexParent, string name) : base(
            parent, parentTransform, zIndexParent, name)
        {
            RectTransform backgroundRectTransform = ZIndexRectTransform;
            
            // create image transform
            CreateUIElement(_SOURCE_IMAGE_NAME, Self, true);
            
            // create image source
            RectTransform sourceImageRectTransform = CreateUIElement(_SOURCE_IMAGE_NAME, backgroundRectTransform, true);

            // add image component
            BackGroundImage = backgroundRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
            Image = sourceImageRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
            
            // add source image tracker
            var sourceImageTracker = sourceImageRectTransform.gameObject.AddComponent<SourceImageTrackerComponent>();
            sourceImageTracker.Initailize(Image);
        }
    }
}
