using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    public interface IElement : IDisposable
    {
        public string Id { get; }
        public string Type { get; }
        public RectTransform Self { get; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool Visible { get; set; }
        public IElement Parent { get; }
        [Obsolete]
        public List<IElement> Children { get; }
        public event EventHandler<Vector2> OnUpdateSize;
        public IElement Copy(RectTransform self, IElement parent);
        public void Update(ElementData data);
    }
}