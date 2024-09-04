using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
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
        public event EventHandler<Vector2> OnUpdateSize;
        public IElement Copy(RectTransform self, IElement parent);
        public void Update(ElementData data);
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