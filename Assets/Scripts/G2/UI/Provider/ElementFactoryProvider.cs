using System;
using System.Collections.Generic;
using G2.UI.Elements.Basic;
using G2.UI.Factory;
using UnityEngine;

namespace G2.UI.Provider
{
    internal class ElementFactoryProvider : FactoryProvider<IElementFactory<Element>>
    {
        public ElementFactoryProvider(Dictionary<string, Sprite> sprites, Vector2 referenceResolution, Action<ulong, string, string, string> onEvent)
        {
            RegisterFactory(new ElementFactory());
            RegisterFactory(new FrameFactory());
            RegisterFactory(new ImageFactory(sprites));
            RegisterFactory(new LabelFactory(referenceResolution));
            RegisterFactory(new ButtonFactory(onEvent));
        }
    }
}
