using System;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Interactive;
using UnityEngine;

namespace G2.UI.Factory
{
    internal class ButtonFactory : BaseElementFactory<ButtonData, ButtonComponents, Button>
    {
        private readonly Action<ulong, string, string, string> _onEvent;

        public ButtonFactory(Action<ulong, string, string, string> onEvent)
        {
            _onEvent = onEvent;
        }

        protected override ButtonComponents CreateComponents(IElement parent, Transform zIndexParent, string name)
        {
            return new ButtonComponents(parent, zIndexParent, name);
        }

        protected override Button CreateTyped(string uid, ButtonData data, ButtonComponents components)
        {
            return new Button(uid, data, components, _onEvent);
        }
    }
}
