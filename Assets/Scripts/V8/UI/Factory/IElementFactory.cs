namespace V8
{
    internal interface IElementFactory<out T> : IFactory where T : IElement
    {
        public T Create(ElementData data, IElement parent);
        // public T Create(ElementData data, IElement parent);
    }
}