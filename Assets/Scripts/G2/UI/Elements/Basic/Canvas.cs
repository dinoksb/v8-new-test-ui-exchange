using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using G2.Model.UI;
using ConstraintType = G2.Model.UI.FrameData.ConstraintType;

namespace G2.UI.Elements.Basic
{
    public class Canvas : IElement
    {
        public string Uid { get; }
        public string Name { get; }

        public RectTransform Self { get; }

        public string Type { get; }

        public Vector2 Size { get; set; }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public bool Visible { get; set; }
        public ConstraintType ConstraintType { get; set; }

        public IElement Parent { get; }
        
        public uint ZIndex { get; }

        public List<IElement> Children { get; private set; } = new();

        event EventHandler<Vector2> IElement.OnSizeUpdated
        {
            add => _onUpdateSizeCore += value;
            remove => _onUpdateSizeCore -= value;
        }

        event EventHandler<Vector2> IElement.OnPositionUpdated
        {
            add => _onPositionUpdatedCore += value;
            remove => _onPositionUpdatedCore -= value;
        }

        event EventHandler<float> IElement.OnRotationUpdated
        {
            add => _onRotationUpdatedCore += value;
            remove => _onRotationUpdatedCore -= value;
        }

        event Action IElement.OnMoveFront
        {
            add => _moveFrontCore += value;
            remove => _moveFrontCore -= value;
        }

        private event EventHandler<Vector2> _onUpdateSizeCore; 
        private event EventHandler<Vector2> _onPositionUpdatedCore; 
        private event EventHandler<float> _onRotationUpdatedCore; 
        private event Action _moveFrontCore; 

        public Canvas(string id, Transform parent, Vector2 resolution, bool dontDestoryOnLoad)
        {
            Name = id;
            Type = GetType().Name;
            var gameObject = new GameObject(Name);

            if (gameObject.transform.parent != parent)
                gameObject.transform.parent = parent;

            var self = gameObject.AddComponent<RectTransform>();
            Self = self;

            var canvas = gameObject.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution =
                resolution == Vector2.zero ? new Vector2(Screen.width, Screen.height) : resolution;
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();
            
            if (dontDestoryOnLoad)
            {
                GameObject.DontDestroyOnLoad(gameObject);
                gameObject.SetActive(false);
            }
        }

        public void Dispose()
        {
            _onUpdateSizeCore = null;
            _moveFrontCore = null;
        }

        public IElement Copy(RectTransform self, IElement parent)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        public void Update(ElementData data)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.MoveFront()
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddVisibleChangedListener(Action<IElement> element)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveVisibleChangedListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveAllVisibleChangedListener()
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddPositionChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemovePositionChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveAllPositionChangeListener()
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddRotationChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveRotationChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveAllRotationChangeListener()
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.AddSizeChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveSizeChangeListener(Action<IElement> action)
        {
            throw new Exception("This function cannot be called in Canvas.");
        }

        void IElement.RemoveAllSizeChangeListener()
        {
            throw new Exception("This function cannot be called in Canvas.");
        }
    }
}
