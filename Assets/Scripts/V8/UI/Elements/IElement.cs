using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    public interface IElement : IDisposable
    {
        public string Name { get; }
        public string Type { get; }
        public RectTransform Self { get; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool Visible { get; set; }
        public IElement Parent { get; }
        public event EventHandler<Vector2> OnUpdateSize;
        public IElement Copy(RectTransform self, IElement parent);
        public void Update(ElementData data);
        internal void AddVisibleChanged(Action<bool> action);
        internal void RemoveVisibleChanged(Action<bool> action);
        internal void AddPositionChange(Action<IElement> action);
        internal void RemovePositionChange(Action<IElement> action);
        internal void AddRotationChange(Action<IElement> action);
        internal void RemoveRotationChange(Action<IElement> action);
        internal void AddSizeChange(Action<IElement> action);
        internal void RemoveSizeChange(Action<IElement> action);
    }
}