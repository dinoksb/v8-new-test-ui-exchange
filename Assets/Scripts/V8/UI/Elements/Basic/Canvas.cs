using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V8
{
    public class Canvas : IElement
    {
        public string Name { get; }

        public RectTransform Self { get; }

        public string Type { get; }

        public Vector2 Size { get; set; }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public bool Visible { get; set; }

        public IElement Parent { get; }

        public List<IElement> Children { get; private set; } = new();

        public event EventHandler<Vector2> OnUpdateSize;

        [Obsolete]
        public Canvas(string id)
        {
            Name = id;
            Type = GetType().Name;
            var gameObject = new GameObject(Name);
            var self = gameObject.AddComponent<RectTransform>();
            Self = self;

            var canvas = gameObject.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();
        }
        
        public Canvas(string id, Transform parent)
        {
            Name = id;
            Type = GetType().Name;
            var gameObject = new GameObject(Name);
            
            if(gameObject.transform.parent != parent) 
                gameObject.transform.parent = parent;
            
            var self = gameObject.AddComponent<RectTransform>();
            Self = self;

            var canvas = gameObject.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();
        }

        public void Dispose()
        {
            OnUpdateSize = null;
        }

        public IElement Copy(RectTransform self, IElement parent)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        public void Update(ElementData data)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }
    }
}