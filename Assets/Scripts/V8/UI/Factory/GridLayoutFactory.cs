namespace V8
{
    internal class GridLayoutFactory : BaseElementFactory<GridLayoutData, GridLayoutComponents, GridLayout>
    {
        protected override GridLayoutComponents CreateComponents(IElement parent, string id)
        {
            return new GridLayoutComponents(parent, id);
        }

        protected override GridLayout CreateTyped(GridLayoutData data, GridLayoutComponents components)
        {
            return new GridLayout(data, components);
        }
    }
}