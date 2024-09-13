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
            var labelRectTransform = Self;
            labelRectTransform.SetParent(Self);
            // labelRectTransform.anchorMin = Vector2.zero;
            // labelRectTransform.anchorMax = Vector2.one;
            // labelRectTransform.offsetMin = Vector2.zero;
            // labelRectTransform.offsetMax = Vector2.zero;

            var labelSource = new GameObject(Self.name);
            var labelSourceRectTransform = labelSource.AddComponent<RectTransform>();
            labelSourceRectTransform.SetParent(zIndexParent);
            
            TransformLinkComponents = labelSource.AddComponent<TransformLinkComponents>().Attach(labelRectTransform);
            TMP = labelSource.gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}