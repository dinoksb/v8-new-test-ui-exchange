namespace V8
{
    public class LayoutComponents : ElementComponents
    {
        public UnityEngine.UI.Image BackgroundImage { get; }

        public LayoutComponents(IElement parent, string id) : base(parent, id)
        {
            BackgroundImage = Self.gameObject.AddComponent<UnityEngine.UI.Image>();
        }
    }
}