using G2.UI.Elements;
using UnityEngine;

namespace G2.Importer
{
    public class DevElementInfo : MonoBehaviour
    {
        public uint ZIndex
        {
            get => _zIndex;
            set => _zIndex = value;
        }
        public IElement Element => _element;
    
        [SerializeField] private uint _zIndex;
        private IElement _element;
    
        public DevElementInfo Attach(IElement element)
        {
            _element = element;
            _zIndex = element.ZIndex;
            return this;
        }
    }
}

