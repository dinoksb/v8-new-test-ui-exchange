using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    internal class ElementFactoryProvider : FactoryProvider<IElementFactory<Element>>
    {
        public ElementFactoryProvider(Dictionary<string, Sprite> sprites, Action<ulong, string, string, string> onEvent)
        {
            RegisterFactory(new ElementFactory());
            RegisterFactory(new LayoutFactory());
            RegisterFactory(new ImageFactory(sprites));
            RegisterFactory(new LabelFactory());
            RegisterFactory(new GridLayoutFactory());
            RegisterFactory(new ButtonFactory(onEvent));
        }
    }
}