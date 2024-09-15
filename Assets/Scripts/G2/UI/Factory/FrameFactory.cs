using UnityEngine;
using G2.Model.UI;
using G2.UI.Elements;
using G2.UI.Elements.Basic;

namespace G2.UI.Factory
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