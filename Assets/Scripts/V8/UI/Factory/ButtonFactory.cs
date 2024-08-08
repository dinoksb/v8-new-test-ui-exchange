using System;

namespace V8
{
    internal class ButtonFactory : BaseElementFactory<ButtonData, ButtonComponents, Button>
    {
        private readonly Action<ulong, string, string, string> _onEvent;

        public ButtonFactory(Action<ulong, string, string, string> onEvent)
        {
            _onEvent = onEvent;
        }

        protected override ButtonComponents CreateComponents(IElement parent)
        {
            return new ButtonComponents(parent);
        }

        protected override Button CreateTyped(ButtonData data, ButtonComponents components)
        {
            return new Button(data, components, _onEvent);
        }
    }
}