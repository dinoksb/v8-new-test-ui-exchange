using UnityEngine;
using UnityEngine.UI;

namespace V8
{
    internal class LabelFactory : BaseElementFactory<LabelData, LabelComponents, Label>
    {
        private readonly Vector2 _referenceResolution;
        public LabelFactory(Vector2 referenceResolution)
        {
            _referenceResolution = referenceResolution;
        }
        
        protected override LabelComponents CreateComponents(IElement parent, Transform zIndexParent, string name)
        {
            return new LabelComponents(parent, zIndexParent, name);
        }

        protected override Label CreateTyped(string uid, LabelData data, LabelComponents components)
        {
            return new Label(uid, data, components, _referenceResolution);
        }
    }
}