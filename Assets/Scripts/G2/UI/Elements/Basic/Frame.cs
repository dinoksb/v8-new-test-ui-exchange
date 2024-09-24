using G2.Model.UI;
using G2.UI.Component;
using UnityEngine;

namespace G2.UI.Elements.Basic
{
    public class Frame : UpdatableElement
    {
        private const string _DIM_NAME = "Dim";
        
        private readonly UnityEngine.UI.Image _dim;
        private static readonly Vector2 _pivotCenter = new(0.5f, 0.5f);

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                if (_dim) _dim.gameObject.SetActive(value);
            }
        }
        
        public Frame(string uid, FrameData data, UpdatableElementComponents components, Transform zIndexParent = null) : base(uid, data, components)
        {
            SetTransformLink(components.ElementTransformLinkComponent);
            if (data.dim == 0) return;
            
            if (zIndexParent != null)
            {
                _dim = CreateDim(zIndexParent, data.dim, Vector3.zero);
                _dim.rectTransform.pivot = _pivotCenter;
                _dim.rectTransform.anchorMin = Vector2.zero;
                _dim.rectTransform.anchorMax = Vector2.one;
            }
            else
            {
                _dim = CreateDim(Self, data.dim, new Vector2(Screen.width, Screen.height));
            }
        }

        private static UnityEngine.UI.Image CreateDim(Transform parent, float opacity, Vector2 size)
        {
            var dimObj = new GameObject(_DIM_NAME);
            dimObj.transform.SetParent(parent);

            var rectTransform = dimObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var image = dimObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, opacity);
            return image;
        }
    }
}
