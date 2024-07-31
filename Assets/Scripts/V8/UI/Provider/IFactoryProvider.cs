namespace V8
{
    internal interface IFactoryProvider<out T>
    {
        public T GetFactory(string type);
    }
}