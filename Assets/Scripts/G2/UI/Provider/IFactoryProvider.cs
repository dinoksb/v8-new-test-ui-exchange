namespace G2.UI.Provider
{
    internal interface IFactoryProvider<out T>
    {
        public T GetFactory(string type);
    }
}