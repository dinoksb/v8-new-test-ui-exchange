using G2.UI.Elements;
using TMPro;
using UnityEngine;

namespace G2.UI.Component
{
    public class LabelComponents : UpdatableElementComponents
    {
        public TMP_Text TMP { get; }

        public LabelComponents(IElement parent, Transform parentTransform, Transform zIndexParent, string name) : base(parent, parentTransform, zIndexParent, name)
        {
            RectTransform labelTransform = ZIndexRectTransform;
            TMP = labelTransform.gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}
