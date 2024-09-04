namespace V8
{
    internal class FrameFactory : BaseElementFactory<FrameData, FrameComponents, Frame>
    {
        protected override FrameComponents CreateComponents(IElement parent, string name)
        {
            return new FrameComponents(parent, name);
        }
        
        protected override Frame CreateTyped(string uid, FrameData data, FrameComponents components)
        {
            return new Frame(uid, data, components);
        }
    }
}