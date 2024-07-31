namespace V8
{
    internal class LayoutFactory : BaseElementFactory<LayoutData, LayoutComponents, Layout>
    {
        protected override LayoutComponents CreateComponents(IElement parent, string id)
        {
            return new LayoutComponents(parent, id);
        }
        
        protected override Layout CreateTyped(LayoutData data, LayoutComponents components)
        {
            return new Layout(data, components);
        }
    }
}