using TMPro;
using UnityEngine;

namespace V8
{
    public class LabelComponents : FrameComponents
    {
        public TMP_Text TMP { get; }
        public TransformLinkComponent TransformLinkComponent { get; }

        public LabelComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            RectTransform labelTransform = Self;
            if (zIndexParent)
            {
                labelTransform = CreateUIElement(Self.name, zIndexParent, false);
            }
            TransformLinkComponent = labelTransform.gameObject.AddComponent<TransformLinkComponent>();
            TMP = labelTransform.gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}