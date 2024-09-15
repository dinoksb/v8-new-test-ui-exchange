using System;
using System.Collections.Generic;
using UnityEngine;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;

namespace G2.UI.Factory
{
    internal class ImageFactory : BaseElementFactory<ImageData, ImageComponents, Image>
    {
        private readonly Dictionary<string, Sprite> _sprites;

        public ImageFactory(Dictionary<string, Sprite> cachedSprites)
        {
            _sprites = cachedSprites ?? new Dictionary<string, Sprite>();
        }

        protected override ImageComponents CreateComponents(IElement parent, Transform zIndexParent, string name)
        {
            return new ImageComponents(parent, zIndexParent, name);
        }

        protected override Image CreateTyped(string uid, ImageData data, ImageComponents components)
        {
            if (!_sprites.TryGetValue(data.spriteId, out var sprite))
            {
                throw new Exception($"'{data.spriteId}' could not be found");
            }

            return new Image(uid, data, components, sprite);
        }
    }
}