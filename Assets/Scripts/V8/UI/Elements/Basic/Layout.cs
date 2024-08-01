using UnityEngine;

namespace V8
{
    // Note: Layout 이 Frame 인것으로 보임.
    public class Layout : Element
    {
        private UnityEngine.UI.Image _backgroundImage;

        public Color BackgroundColor
        {
            get => _backgroundImage.color;
            set => _backgroundImage.color = value;
        }

        public Layout(LayoutData data, LayoutComponents components) : base(data, components)
        {
            _backgroundImage = components.BackgroundImage;
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Layout)base.Copy(self, parent);
            clone._backgroundImage = self.GetComponent<UnityEngine.UI.Image>();
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var layoutData = (LayoutData)data;
            SetValues(layoutData);
        }

        private void SetValues(LayoutData data)
        {
            BackgroundColor = TypeConverter.ToColor(data.backgroundColor);
        }
    }
}