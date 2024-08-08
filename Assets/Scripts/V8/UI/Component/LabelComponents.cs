using TMPro;
using UnityEngine;

namespace V8
{
    public class LabelComponents : FrameComponents
    {
        public TMP_Text TMP { get; }

        public LabelComponents(IElement parent, string name) : base(parent, name)
        {
            var go = new GameObject(UIConfig.Element);
            var element = go.AddComponent<RectTransform>();
            element.SetParent(Self);
            element.anchorMin = Vector2.zero;
            element.anchorMax = Vector2.one;
            element.offsetMin = Vector2.zero;
            element.offsetMax = Vector2.zero;

            TMP = go.AddComponent<TextMeshProUGUI>();
        }
    }
}