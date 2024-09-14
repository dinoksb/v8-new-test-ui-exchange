using TMPro;
using UnityEngine;

namespace V8
{
    public class LabelComponents : FrameComponents
    {
        public TMP_Text TMP { get; }
        public TransformLinkComponents TransformLinkComponents { get; }

        public LabelComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
            var labelSource = new GameObject(Self.name);
            var labelSourceRectTransform = labelSource.AddComponent<RectTransform>();
            labelSourceRectTransform.SetParent(zIndexParent);

            TransformLinkComponents = labelSource.AddComponent<TransformLinkComponents>();
            TMP = labelSource.gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}