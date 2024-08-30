using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConstraintType = V8.FrameData.ConstraintType;

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
        public ConstraintType ConstraintType { get; set; }

        public IElement Parent { get; }

        public List<IElement> Children { get; private set; } = new();

        public event EventHandler<Vector2> OnUpdateSize;
        
        public Canvas(string id, Transform parent, Vector2 resolution)
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
            canvasScaler.referenceResolution = resolution == Vector2.zero ? new Vector2(Screen.width, Screen.height) : resolution;
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

        void IElement.AddVisibleChanged(Action<bool> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveVisibleChanged(Action<bool> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddPositionChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemovePositionChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddRotationChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveRotationChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddSizeChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveSizeChange(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }
    }
}