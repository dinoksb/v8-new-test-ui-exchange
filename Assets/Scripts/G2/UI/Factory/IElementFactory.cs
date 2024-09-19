using UnityEngine;
using G2.Model.UI;
using G2.UI.Elements;

namespace G2.UI.Factory
{
    internal interface IElementFactory<out T> : IFactory
    {
        public T Create(string uid, ElementData data, IElement parent, Transform zIndexParent);
    }
}
