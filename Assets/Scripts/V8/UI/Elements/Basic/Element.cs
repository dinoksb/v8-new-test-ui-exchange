using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace V8
{
    public class Element : IElement
    {
        private bool _isOnUpdateSizeSubscribed;

        public string Id { get; }

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

        public Vector2 WorldPosition
        {
            get => Self.position;
            set => Self.position = value;
        }

        public bool Visible
        {
            get => Self.gameObject.activeSelf;
            set => Self.gameObject.SetActive(value);
        }

        public IElement Parent { get; private set; }

        public List<IElement> Children { get; private set; } = new();

        public event EventHandler<Vector2> OnUpdateSize;

        public Element(ElementData data, ElementComponents components)
        {
            Id = data.id;
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

            clone.Children = new List<IElement>();
            foreach (var child in Children)
            {
                var childSelf = GetChildSelf(self, child.Id);
                var newChild = child.Copy(childSelf, clone);
                clone.Children.Add(newChild);
            }

            return clone;
        }


        public virtual void Update(ElementData data)
        {
            SetValues(data);
            foreach (var childData in data.children)
            {
                var childElement = Children.FirstOrDefault(x => x.Id == childData.id);
                childElement?.Update(childData);
            }
            
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
            AnchorMin = TypeConverter.ToVector2(data.anchorMin);
            AnchorMax = TypeConverter.ToVector2(data.anchorMax);
            Pivot = TypeConverter.ToVector2(data.pivot);
            Size = CalculateSize(data.size);
            if (UIConfig.Relative == data.positionLayout)
            {
                Position = CalculatePosition(data.position, true);
            }
            else
            {
                WorldPosition = CalculatePosition(data.position, false);
            }
        }

        private Vector2 CalculatePosition(IReadOnlyList<string> data, bool relative)
        {
            var x = ConvertToUnits(data[0], relative);
            var y = ConvertToUnits(data[1], relative);
            return new Vector2(x, y);
        }

        private Vector2 CalculateSize(IReadOnlyList<string> data)
        {
            if (data[0].Equals(UIConfig.Parent))
            {
                _isOnUpdateSizeSubscribed = true;
                Parent.OnUpdateSize += UpdateSize;
                return Parent.Size;
            }

            var width = ConvertToUnits(data[0]);
            var height = ConvertToUnits(data[1]);
            return new Vector2(width, height);
        }

        private void UpdateSize(object _, Vector2 size)
        {
            Size = size;
        }

        private float ConvertToUnits(string value, bool relative = false)
        {
            if (value.EndsWith(UIConfig.Width))
            {
                var ratio = TypeConverter.ToRatio(value.TrimEnd(UIConfig.Width));
                return (relative ? Parent.Self.rect.width : Screen.width) * ratio;
            }

            if (value.EndsWith(UIConfig.Height))
            {
                var ratio = TypeConverter.ToRatio(value.TrimEnd(UIConfig.Height));
                return (relative ? Parent.Self.rect.height : Screen.height) * ratio;
            }

            return TypeConverter.ToFloat(value);
        }
    }
}