using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    public class Element : IElement
    {
        private bool _isOnUpdateSizeSubscribed;
        public string Name { get; }

        public string Type { get; }

        public RectTransform Self { get; private set; }

        public Vector2 AnchorMin
        {
            get => Self.anchorMin;
            set => Self.anchorMin = value;
        }

        public Vector2 AnchorMax
        {
            get => Self.anchorMax;
            set => Self.anchorMax = value;
        }

        public Vector2 Pivot
        {
            get => Self.pivot;
            set => Self.pivot = value;
        }

        public Vector2 Size
        {
            get => Self.sizeDelta;
            set
            {
                Self.sizeDelta = value;
                OnUpdateSize?.Invoke(this, value);
            }
        }

        public Vector2 Position
        {
            get => Self.anchoredPosition;
            set => Self.anchoredPosition = value;
        }

        public float Rotation
        {
            get => Self.eulerAngles.z;
            set
            {
                float deltaZ = value - Self.eulerAngles.z;
                Self.Rotate(0, 0, deltaZ);
            }
        }

        public bool Visible
        {
            get => Self.gameObject.activeSelf;
            set => Self.gameObject.SetActive(value);
        }

        public IElement Parent { get; private set; }

        public event EventHandler<Vector2> OnUpdateSize;

        public Element(ElementData data, ElementComponents components)
        {
            Name = data.name;
            Type = data.type;
            Self = components.Self;
            Parent = components.Parent;
            SetValues(data);
        }

        public void Dispose()
        {
            OnUpdateSize = null;
        }

        public virtual IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Element)MemberwiseClone();
            clone.Self = self;
            clone.Parent = parent;
            
            if (clone._isOnUpdateSizeSubscribed)
            {
                clone.Parent.Dispose();
                clone.Parent.OnUpdateSize += clone.UpdateSize;
            }

            return clone;
        }


        public virtual void Update(ElementData data)
        {
            SetValues(data);
            
            Visible = data.visible;
        }

        protected static RectTransform GetChildSelf(Transform targetSelf, string id)
        {
            for (var i = 0; i < targetSelf.childCount; i++)
            {
                var newChild = targetSelf.GetChild(i);
                if (newChild.name == id)
                {
                    return newChild.GetComponent<RectTransform>();
                }
            }

            throw new Exception($"No child with ID '{id}' found under {targetSelf.name}.");
        }

        private void SetValues(ElementData data)
        {
            AnchorMin = SetRectTransformAnchorPivot(data.anchorMin);
            AnchorMax = SetRectTransformAnchorPivot(data.anchorMax);
            Pivot = SetRectTransformAnchorPivot(data.pivot);
            Size = data.size;
            Position = data.position;
            Rotation = data.rotation;

            Vector2 SetRectTransformAnchorPivot(IReadOnlyList<float> values)
            {
                var convertedValue = TypeConverter.ToVector2(values);
                return convertedValue == Vector2.zero ? new Vector2(0.5f, 0.5f) : convertedValue;
            }
        }

        private void UpdateSize(object _, Vector2 size)
        {
            Size = size;
        }
    }
}