using System;
using UnityEngine;
using G2.Model.UI;

namespace G2.UI.Elements
{
    public enum ElementType
    {
        Frame,
        Image,
        Label,
        Button
    }
    
    public interface IElement : IDisposable
    {
        public string Uid { get; }
        public string Name { get; }
        public string Type { get; }
        public RectTransform Self { get; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool Visible { get; set; }
        public IElement Parent { get; }
        public uint ZIndex { get; }
        public IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement);
        public void Update(ElementData data);
        internal event EventHandler<Vector2> OnSizeUpdated;
        internal event EventHandler<Vector2> OnPositionUpdated;
        internal event EventHandler<float> OnRotationUpdated;
        internal void AddVisibleChangedListener(Action<IElement> action);
        internal void RemoveVisibleChangedListener(Action<IElement> action);
        internal void RemoveAllVisibleChangedListener();
        internal void AddPositionChangeListener(Action<IElement> action);
        internal void RemovePositionChangeListener(Action<IElement> action);
        internal void RemoveAllPositionChangeListener();
        internal void AddRotationChangeListener(Action<IElement> action);
        internal void RemoveRotationChangeListener(Action<IElement> action);
        internal void RemoveAllRotationChangeListener();
        internal void AddSizeChangeListener(Action<IElement> action);
        internal void RemoveSizeChangeListener(Action<IElement> action);
        internal void RemoveAllSizeChangeListener();
    }
}
