using UnityEngine.UI;

namespace V8
{
    public class GridLayoutComponents : LayoutComponents
    {
        public GridLayoutGroup GridLayout { get; }
        
        public GridLayoutComponents(IElement parent, string id) : base(parent, id)
        {
            GridLayout = Self.gameObject.AddComponent<GridLayoutGroup>();
        }
    }
}