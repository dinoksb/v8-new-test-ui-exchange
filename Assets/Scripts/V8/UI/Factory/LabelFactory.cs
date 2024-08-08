namespace V8
{
    internal class LabelFactory : BaseElementFactory<LabelData, LabelComponents, Label>
    {
        protected override LabelComponents CreateComponents(IElement parent)
        {
            return new LabelComponents(parent);
        }

        protected override Label CreateTyped(LabelData data, LabelComponents components)
        {
            return new Label(data, components);
        }
    }
}