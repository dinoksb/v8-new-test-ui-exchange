namespace V8
{
    internal class ElementFactory : BaseElementFactory<ElementData, ElementComponents, Element>
    {
        protected override ElementComponents CreateComponents(IElement parent, string id)
        {
            return new ElementComponents(parent, id);
        }

        protected override Element CreateTyped(ElementData data, ElementComponents components)
        {
            return new Element(data, components);
        }
    }
}