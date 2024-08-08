using UnityEngine;

namespace V8
{
    public class Frame : Element
    {
        public bool interactable;
        public Frame(FrameData data, FrameComponents components) : base(data, components)
        {
            SetValues(data);
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var layoutData = (FrameData)data;
            SetValues(layoutData);
        }

        private void SetValues(FrameData data)
        {
            interactable = data.interactable;
        }
    }
}