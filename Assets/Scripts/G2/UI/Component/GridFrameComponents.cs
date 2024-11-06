using G2.UI.Elements;
using UnityEngine;

namespace G2.UI.Component
{
    
    public class GridFrameComponents : FrameComponents
    {
        public UnityEngine.UI.ScrollRect ScrollRect { get; }
        public UnityEngine.UI.Image BackGroundImage { get; }
        
        private const string _VIEWPORT_NAME = "Viewport";
        private const string _CONTENT_NAME = "Content";
        private const string _SCROLLBAR_NAME = "Scrollbar";
        private const string _SCROLLBAR_AREA_NAME = "ScrollbarArea";
        private const string _SCROLLBAR_HANDLE_NAME = "Handle";

        public GridFrameComponents(IElement parent, Transform zIndexParent, string name) :
            base(parent, zIndexParent, name)
        {
            // TODO: Need to create the objects required to configure the ScrollRect.
            
            ScrollRect = GUIRectTransform.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
            BackGroundImage = GUIRectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
            
            // create view port
            var viewPort = CreateUIElement(_VIEWPORT_NAME, Self, true);
            var contentPivot = new Vector2(0.5f, 1);
            var content = CreateUIElement(_CONTENT_NAME, viewPort, Vector2.up, Vector2.one, Vector2.zero, Vector2.zero, contentPivot);
            
            // create vertical slider
            
            // TODO: set vertical scroll width from data 
            var verticalScrollRect = CreateUIElement($"{_SCROLLBAR_NAME}_Vertical", Self, Vector2.right, Vector2.one,
                new Vector2(-20, 0), Vector2.zero, Vector2.one);
            var verticalScrollArea = CreateUIElement(_SCROLLBAR_AREA_NAME, verticalScrollRect, true);
            var verticalScrollAreaWidth = verticalScrollRect.rect.width * 0.5f;
            var verticalScrollAreaOffset = new Vector2(verticalScrollAreaWidth, verticalScrollAreaWidth);
            verticalScrollArea.offsetMin = verticalScrollAreaOffset;
            verticalScrollArea.offsetMax = -verticalScrollAreaOffset;
            var verticalScrollHandle = CreateUIElement(_SCROLLBAR_HANDLE_NAME, verticalScrollArea, Vector2.zero,
                new Vector2(1, 0.5f), -verticalScrollAreaOffset, -verticalScrollAreaOffset, new Vector2(0.5f, 0.5f));
            
            // create horizontal slider
            // var horizontalScrollRect = CreateUIElement($"{_SCROLLBAR_NAME}_Horizontal", Self, Vector2.zero, Vector2.right,
            //     Vector2.zero, Vector2.zero, Vector2.zero);
            // var horizontalScrollArea = CreateUIElement(_SCROLLBAR_AREA_NAME, horizontalScrollRect, true);
            // horizontalScrollArea.rect.min = new
            //
            // // create vertical slider
            // var verticalScrollRect = CreateUIElement($"{_SCROLLBAR_NAME}_Vertical", Self, Vector2.right, Vector2.one,
            //     Vector2.zero, Vector2.zero, Vector2.one);
        }
    }
}
