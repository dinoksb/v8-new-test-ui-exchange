using System;
using System.Collections.Generic;

namespace V8
{
    internal abstract class FactoryProvider<T> : IFactoryProvider<T> where T : IFactory
    {
        private readonly Dictionary<string, T> _factories = new();

        public T GetFactory(string type)
        {
            if (!_factories.TryGetValue(type, out var factory))
            {
                throw new Exception($"Factory for type {type} is not implemented.");
            }

            return factory;
        }

        protected void RegisterFactory(T factory)
        {
            _factories[factory.Type] = factory;
        }
    }
}