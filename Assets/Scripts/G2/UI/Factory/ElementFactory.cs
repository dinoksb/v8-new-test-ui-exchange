using System;
using G2.Model.UI;
using G2.UI.Component;
using G2.UI.Elements;
using G2.UI.Elements.Basic;
using G2.UI.Elements.Interactive;
using UnityEngine;

namespace G2.UI
{
    public static class ElementFactory
    {
        public static Frame CreateFrame(string uid, IElement parentElement, Transform parentTransform, Transform zIndexTransform, FrameData data)
        {
            var components = new UpdatableElementComponents(parentElement, parentTransform, zIndexTransform, data.Name);
            return new Frame(uid, data, components, zIndexTransform);
        }

        public static Image CreateImage(string uid, IElement parentElement, Transform parentTransform, Transform zIndexTransform, ImageData data, Sprite sprite)
        {
            var components = new ImageComponents(parentElement, parentTransform, zIndexTransform, data.Name);
            return new Image(uid, data, components, sprite);
        }

        public static Label CreateLabel(string uid, IElement parentElement, Transform parentTransform, Transform zIndexTransform, LabelData data, Vector2 referenceResolution)
        {
            var components = new LabelComponents(parentElement, parentTransform, zIndexTransform, data.Name);
            return new Label(uid, data, components, referenceResolution);
        }

        public static Button CreateButton(string uid, IElement parentElement, Transform parentTransform, Transform zIndexTransform, ButtonData data, Action<ulong, string, string, string> onEvent)
        {
            var components = new ButtonComponents(parentElement, parentTransform, zIndexTransform, data.Name);
            return new Button(uid, data, components, onEvent);
        }
    }
}
