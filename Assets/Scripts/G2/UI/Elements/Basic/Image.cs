using System.Collections.Generic;
using UnityEngine;
using G2.Model.UI;
using G2.UI.Component;
using Utilities;

namespace G2.UI.Elements.Basic
{
    // Todo: Add source image change functionality and raise event functionality.
    // Todo: Add source image release functionality.
    public class Image : UpdatableElement
    {
        private readonly UnityEngine.UI.Image _backgroundImage;
        private readonly List<ColorChangeEventAction> _backgroundColorChangeEvents = new();
        private readonly List<ColorChangeEventAction> _sourceColorChangeEvents = new();
        private readonly List<SourceChangeEventAction> _sourceChangeEvents = new();
        
        private UnityEngine.UI.Image _image;
        private TransformLinkComponent _elementTransformLink;

        public delegate void ColorChangeEventAction(IElement element, Color prevColor, Color newColor);

        public delegate void SourceChangeEventAction(IElement element, string prevUrl, string newUrl);

        public override bool Interactable
        {
            get => _image.raycastTarget;
            set
            {
                _backgroundImage.raycastTarget = value;
                _image.raycastTarget = value;
            }
        }
        
        public Color BackgroundColor
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

        public Image(string uid, ImageData data, ImageComponents components, Sprite sprite) : base(uid, data, components)
        {
            _backgroundImage = components.BackGroundImage;
            _image = components.Image;
            _image.type = UnityEngine.UI.Image.Type.Sliced;
            _image.sprite = sprite;
            _elementTransformLink = components.ElementTransformLinkComponent;
            SetTransformLink(_elementTransformLink);
            SetEvents();
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement)
        {
            // todo: Modify it so that instead of finding the sourceImage in the image by its name, it finds it by attaching a component called sourceImage.
            var clone = (Image)base.Copy(self, parentRectTransform, parentElement);
            var sourceImageTracker = _elementTransformLink.Self.GetComponentInChildren<SourceImageTrackerComponent>();
            clone._image = sourceImageTracker.SourceImage;
            if (_elementTransformLink)
            {
                clone._elementTransformLink = _elementTransformLink;
                clone.SetTransformLink(clone._elementTransformLink);
            }

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
            BackgroundColor = TypeConverter.ToColor(data.backgroundColor);
            ImageColor = TypeConverter.ToColor(data.imageColor);
            Interactable = data.interactable;
        }


        private void SetEvents()
        {
            if (Parent == null) return;

            Parent.OnPositionUpdated += PositionChanged;
            Parent.AddVisibleChangedListener(VisibleChanged);
        }

        public void AddBackgroundColorChangedListener(ColorChangeEventAction eventAction)
        {
            _backgroundColorChangeEvents.Add(eventAction);
        }

        public void RemoveBackgroundColorChangedListener(ColorChangeEventAction eventAction)
        {
            _backgroundColorChangeEvents.Remove(eventAction);
        }

        public void RemoveAllBackgroundColorChangedListener()
        {
            _backgroundColorChangeEvents.Clear();
        }

        public void AddSourceColorChangedListener(ColorChangeEventAction eventAction)
        {
            _sourceColorChangeEvents.Add(eventAction);
        }

        public void RemoveSourceColorChangedListener(ColorChangeEventAction eventAction)
        {
            _sourceColorChangeEvents.Remove(eventAction);
        }

        public void RemoveAllSourceColorChangedListener()
        {
            _sourceColorChangeEvents.Clear();
        }

        public void AddSourceChangedListener(SourceChangeEventAction eventAction)
        {
            _sourceChangeEvents.Add(eventAction);
        }

        public void RemoveSourceChangedListener(SourceChangeEventAction eventAction)
        {
            _sourceChangeEvents.Remove(eventAction);
        }

        public void RemoveAllSourceChangedListener()
        {
            _sourceChangeEvents.Clear();
        }
    }
}
