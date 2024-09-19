using System.Collections.Generic;
using UnityEngine;
using G2.Model.UI;
using Utilities;

namespace G2.UI.Elements.Basic
{
    // Todo: source Image 변경 기능 및 Raise Event 기능 추가
    // Todo: source Image Release 기능 추가
    public class Image : Frame
    {
        public override bool Interactable
        {
            get => _image.raycastTarget;
            set
            {
                _backgroundImage.raycastTarget = value;
                _image.raycastTarget = value;
            }
        }

        private UnityEngine.UI.Image _backgroundImage;
        private UnityEngine.UI.Image _image;
        private TransformLinkComponent _transformLink;

        //Todo: Lazy 초기화가 필요할지 고민필요...
        private readonly List<ColorChanageEventAction> _backgroundColorChangeEvents = new();
        private readonly List<ColorChanageEventAction> _sourceColorChangeEvents = new();
        private readonly List<SorceChangeEventAction> _sourceChangeEvents = new();

        public delegate void ColorChanageEventAction(IElement element, Color prevColor, Color newColor);

        public delegate void SorceChangeEventAction(IElement element, string prevUrl, string newUrl);

        public Color BackgroudColor
        {
            get => _backgroundImage.color;
            set
            {
                var prevColor = _backgroundImage.color;
                _backgroundImage.color = value;
                foreach (var eventAction in _backgroundColorChangeEvents)
                {
                    eventAction?.Invoke(this, prevColor, value);
                }
            }
        }

        public Color ImageColor
        {
            get => _image.color;
            set
            {
                var prevColor = _image.color;
                _image.color = value;
                foreach (var eventAction in _sourceColorChangeEvents)
                {
                    eventAction?.Invoke(this, prevColor, value);
                }
            }
        }

        public Image(string uid, ImageData data, ImageComponents components, Sprite sprite) : base(uid, data,
            components)
        {
            _backgroundImage = components.BackGroundImage;
            _image = components.Image;
            _image.type = UnityEngine.UI.Image.Type.Sliced;
            _image.sprite = sprite;
            _transformLink = components.TransformLinkComponent;
            SetTransformLink(_transformLink);
            SetEvents();
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            // todo: image 에서 sourceImage 를 이름으로 찾는것이 아니라 sourceImage 라는 컴포넌트를 붙여서 찾는 방식으로 수정
            var clone = (Image)base.Copy(self, parent);
            var childSelf = GetChildSelf(self, UIConfig.ImageSource);
            clone._image = childSelf.GetComponent<UnityEngine.UI.Image>();
            if (_transformLink)
            {
                clone._transformLink = _transformLink;
                clone.SetTransformLink(clone._transformLink);
            }

            clone.Parent.OnMoveFront += clone.MoveFront;
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var imageData = (ImageData)data;
            SetValues(imageData);
        }

        public override void MoveFront()
        {
            _transformLink.Self.SetAsLastSibling();
            base.MoveFront();
        }

        private void SetValues(ImageData data)
        {
            BackgroudColor = TypeConverter.ToColor(data.backgroundColor);
            ImageColor = TypeConverter.ToColor(data.imageColor);
            Interactable = data.interactable;
        }


        private void SetEvents()
        {
            if (CheckIsCanvas(Parent)) return;
            
            Parent.OnMoveFront += MoveFront;
            Parent.OnPositionUpdated += PositionChanged;
            Parent.AddVisibleChangedListener(VisibleChanged);
        }

        public void AddBackgroundColorChangedListener(ColorChanageEventAction eventAction)
        {
            _backgroundColorChangeEvents.Add(eventAction);
        }

        public void RemoveBackgroundColorChangedListener(ColorChanageEventAction eventAction)
        {
            _backgroundColorChangeEvents.Remove(eventAction);
        }

        public void RemoveAllBackgroundColorChangedListener()
        {
            _backgroundColorChangeEvents.Clear();
        }

        public void AddSorceColorChangedListener(ColorChanageEventAction eventAction)
        {
            _sourceColorChangeEvents.Add(eventAction);
        }

        public void RemoveSourceColorChangedListener(ColorChanageEventAction eventAction)
        {
            _sourceColorChangeEvents.Remove(eventAction);
        }

        public void RemoveAllSourceColorChangedListener()
        {
            _sourceColorChangeEvents.Clear();
        }

        public void AddSorceChangedListener(SorceChangeEventAction eventAction)
        {
            _sourceChangeEvents.Add(eventAction);
        }

        public void RemoveSourceChangedListener(SorceChangeEventAction eventAction)
        {
            _sourceChangeEvents.Remove(eventAction);
        }

        public void RemoveAllSourceChangedListener()
        {
            _sourceChangeEvents.Clear();
        }
    }
}
