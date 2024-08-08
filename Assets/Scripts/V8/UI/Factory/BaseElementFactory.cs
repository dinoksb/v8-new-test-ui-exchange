using System;

namespace V8
{
    internal abstract class BaseElementFactory<TData, TComponents, TElement> : IElementFactory<TElement>
        where TData : ElementData
        where TComponents : ElementComponents
        where TElement : Element

    {
        public string Type => typeof(TElement).Name;
     
        public TElement Create(ElementData data, IElement parent)
        {
            if (data is not TData typedData)
            {
                throw new InvalidOperationException($"Data is not of the expected type '{typeof(TData).Name}'.");
            }

            var typedComponents = CreateComponents(parent, data.name);
            return CreateTyped(typedData, typedComponents);
        }

        protected abstract TComponents CreateComponents(IElement parent, string name);
        
        protected abstract TElement CreateTyped(TData data, TComponents components);
    }
}