namespace V8
{
    internal class FrameFactory : BaseElementFactory<FrameData, FrameComponents, Frame>
    {
        protected override FrameComponents CreateComponents(IElement parent, string id)
        {
            return new FrameComponents(parent, id);
        }
        
        protected override Frame CreateTyped(FrameData data, FrameComponents components)
        {
            return new Frame(data, components);
        }
    }
}