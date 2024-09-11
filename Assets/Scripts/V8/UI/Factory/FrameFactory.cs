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
            bool isUseDim = _dimOpacity != 0;
            return new FrameComponents(parent, name, isUseDim);
        }
        
        protected override Frame CreateTyped(string uid, FrameData data, FrameComponents components)
        {
            return new Frame(uid, data, components, _dimOpacity, _referenceResolution);
        }
    }
}