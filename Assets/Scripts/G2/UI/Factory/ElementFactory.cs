using System;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;
using G2.UI.Elements.Interactive;
using UnityEngine;

namespace G2.UI
{
    public static class ElementFactory
    {
        public static Frame CreateFrame(string uid, IElement parent, Transform zIndexParent, FrameData data)
        {
            var components = new FrameComponents(parent, zIndexParent, data.name);
            return new Frame(uid, data, components);
        }

        public static Image CreateImage(string uid, IElement parent, Transform zIndexParent, ImageData data, Sprite sprite)
        {
            var components = new ImageComponents(parent, zIndexParent, data.name);
            return new Image(uid, data, components, sprite);
        }

        public static Label CreateLabel(string uid, IElement parent, Transform zIndexParent, LabelData data, Vector2 referenceResolution)
        {
            var components = new LabelComponents(parent, zIndexParent, data.name);
            return new Label(uid, data, components, referenceResolution);
        }

        public static Button CreateButton(string uid, IElement parent, Transform zIndexParent, ButtonData data, Action<ulong, string, string, string> onEvent)
        {
            var components = new ButtonComponents(parent, zIndexParent, data.name);
            return new Button(uid, data, components, onEvent);
        }
    }
}
