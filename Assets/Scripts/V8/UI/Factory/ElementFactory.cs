using UnityEngine;

namespace V8
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