using UnityEngine;

namespace V8
{
    internal interface IElementFactory<out T> : IFactory where T : IElement
    {
        public T Create(string uid, ElementData data, IElement parent, Transform zIndexParent);
    }
}