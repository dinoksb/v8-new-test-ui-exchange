using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;
using UnityEngine;

namespace G2.UI.Factory
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
