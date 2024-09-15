using UnityEngine;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;

namespace G2.UI.Factory
{
    internal class ElementFactory : BaseElementFactory<ElementData, ElementComponents, Element>
    {
        protected override ElementComponents CreateComponents(IElement parent, Transform zIndexParent, string name)
        {
            return new ElementComponents(parent, zIndexParent, name);
        }

        protected override Element CreateTyped(string uid, ElementData data, ElementComponents components)
        {
            return new Element(uid, data, components);
        }
    }
}