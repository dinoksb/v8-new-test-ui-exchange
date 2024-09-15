using UnityEngine;

namespace V8
{
    internal class FrameFactory : BaseElementFactory<FrameData, FrameComponents, Frame>
    {
        private Transform _zIndexParent;
        
        protected override FrameComponents CreateComponents(IElement parent, Transform zIndexParent, string name)
        {
            _zIndexParent = zIndexParent;
            return new FrameComponents(parent, zIndexParent, name);
        }
        
        protected override Frame CreateTyped(string uid, FrameData data, FrameComponents components)
        {
            return new Frame(uid, data, components, _zIndexParent);
        }
    }
}