using UnityEngine;

namespace V8
{
    public class FrameComponents : ElementComponents
    {
        public FrameComponents(IElement parent, Transform zIndexParent, string name) : base(parent, zIndexParent, name)
        {
        }

        // public FrameComponents(IElement parent, Transform zIndexParent, string name, bool isUseDim) : base(parent, zIndexParent, name)
        // {
        //     if (!isUseDim) return;
        //     
        //     GameObject go = new GameObject(UIConfig.DimType);
        //     go.transform.SetParent(zIndexParent);
        //     go.layer = LayerMask.NameToLayer(UIConfig.LayerName);
        //     
        //     var rectTransform = go.AddComponent<RectTransform>();
        //     rectTransform.anchoredPosition = Vector2.zero;
        //
        //     var image = go.AddComponent<UnityEngine.UI.Image>();
        //     image.color = new Color(0, 0, 0, 0);
        //
        //     Dim = image;
        // }
    }
}