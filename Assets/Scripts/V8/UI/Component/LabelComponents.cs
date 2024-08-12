using TMPro;
using UnityEngine;

namespace V8
{
    public class LabelComponents : FrameComponents
    {
        public TMP_Text TMP { get; }

        public LabelComponents(IElement parent, string name) : base(parent, name)
        {
            var element = Self;
            element.SetParent(Self);
            element.anchorMin = Vector2.zero;
            element.anchorMax = Vector2.one;
            element.offsetMin = Vector2.zero;
            element.offsetMax = Vector2.zero;

            TMP = element.gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}