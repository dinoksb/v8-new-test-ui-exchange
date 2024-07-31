using UnityEngine;

namespace V8
{
    public class Image : Layout
    {
        private UnityEngine.UI.Image _image;

        public Color ImageColor
        {
            get => _image.color;
            set => _image.color = value;
        }

        public Image(ImageData data, ImageComponents components, Sprite sprite) : base(data, components)
        {
            _image = components.Image;
            _image.type = UnityEngine.UI.Image.Type.Sliced; 
            _image.sprite = sprite;
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Image)base.Copy(self, parent);
            var childSelf = GetChildSelf(self, UIConfig.Element);
            clone._image = childSelf.GetComponent<UnityEngine.UI.Image>();
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var imageData = (ImageData)data;
            SetValues(imageData);
        }

        private void SetValues(ImageData data)
        {
            ImageColor = TypeConverter.ToColor(data.imageColor);
        }
    }
}