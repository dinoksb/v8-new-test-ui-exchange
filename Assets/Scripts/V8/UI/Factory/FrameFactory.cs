using UnityEngine;

namespace V8
{
    internal class FrameFactory : BaseElementFactory<FrameData, FrameComponents, Frame>
    {
        private readonly float _dimOpacity;
        private readonly Vector2 _referenceResolution;
        
        public FrameFactory(float dimOpacity, Vector2 referenceResolution)
        {
            _dimOpacity = dimOpacity;
            _referenceResolution = referenceResolution;
        }
        
        protected override FrameComponents CreateComponents(IElement parent, string name)
        {
            return new FrameComponents(parent, name, _dimOpacity, _referenceResolution);
        }
        
        protected override Frame CreateTyped(string uid, FrameData data, FrameComponents components)
        {
            return new Frame(uid, data, components);
        }
    }
}