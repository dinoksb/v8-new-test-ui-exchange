using UnityEngine;
using UnityEngine.UI;

namespace V8
{
    internal class LabelFactory : BaseElementFactory<LabelData, LabelComponents, Label>
    {
        private readonly CanvasScaler _canvasScaler;
        public LabelFactory(CanvasScaler canvasScaler)
        {
            _canvasScaler = canvasScaler;
        }
        
        protected override LabelComponents CreateComponents(IElement parent, string name)
        {
            return new LabelComponents(parent, name);
        }

        protected override Label CreateTyped(LabelData data, LabelComponents components)
        {
            return new Label(data, components, _canvasScaler);
        }
    }
}