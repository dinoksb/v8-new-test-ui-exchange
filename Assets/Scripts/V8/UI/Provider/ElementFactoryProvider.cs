using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    internal class ElementFactoryProvider : FactoryProvider<IElementFactory<Element>>
    {
        public ElementFactoryProvider(Dictionary<string, Sprite> sprites, Vector2 referenceResolution, float dimOpacity, Action<ulong, string, string, string> onEvent)
        {
            RegisterFactory(new ElementFactory());
            RegisterFactory(new FrameFactory(dimOpacity, referenceResolution));
            RegisterFactory(new ImageFactory(sprites));
            RegisterFactory(new LabelFactory(referenceResolution));
            RegisterFactory(new ButtonFactory(onEvent));
        }
    }
}