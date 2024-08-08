namespace V8
{
    internal class FrameFactory : BaseElementFactory<FrameData, FrameComponents, Frame>
    {
        protected override FrameComponents CreateComponents(IElement parent)
        {
            return new FrameComponents(parent);
        }
        
        protected override Frame CreateTyped(FrameData data, FrameComponents components)
        {
            return new Frame(data, components);
        }
    }
}