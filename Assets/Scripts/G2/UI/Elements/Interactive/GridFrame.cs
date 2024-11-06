using System.Collections.Generic;
using G2.Model.UI;
using G2.UI.Component;
using G2.UI.Elements.Basic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace G2.UI.Elements.Interactive
{
    public class GridFrame : Frame
    {
        private bool _horizontal;
        public bool Horizontal
        {
            get => _horizontal;
            set
            {
                if (value != _horizontal)
                {
                    _horizontal = value;
                    _vertical = !_horizontal;
                    _scrollRect.horizontal = value;
                }
            }
        }

        private bool _vertical;
        public bool Vertical
        {
            get => _scrollRect.vertical;
            set
            {
                if (value != _vertical)
                {
                    _vertical = value;
                    _horizontal = !_vertical;
                    _scrollRect.vertical = value;
                }
            }
        }

        private int itemConstraintCount;
        public int ItemConstraintCount
        {
            get => itemConstraintCount;
            set => itemConstraintCount = value;
        }
        

        // public ChangeListenerManager<ScrollFrame, Vector2> ScrollValueChangedListeners { get; } = new();
        
        private readonly List<IElement> _items;
        private ScrollRect _scrollRect;
        private GridLayoutGroup _gridLayout;
        private Vector2 _scrollValue;
        
        public GridFrame(string uid, GridFrameData data, GridFrameComponents components) : base(uid, data, components)
        {
            _scrollRect.onValueChanged.AddListener(HandleScrollValueChanged);
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var scrollRectData = (GridFrameData)data;
            SetValues(scrollRectData);
        }

        /// <summary>
        /// add content by element type
        /// </summary>
        /// <param name="element">ui element</param>
        public void Add(IElement element)
        {
            // TODO: feature implementation required.
            _items.Add(element);
        }

        /// <summary>
        /// remove content by element type
        /// </summary>
        /// <param name="element">ui element</param>
        public void Remove(IElement element)
        {
            // TODO: feature implementation required.
            _items.Remove(element);
        }

        private void SetValues(GridFrameData data)
        {
            Interactable = data.interactable;
        }

        private void HandleScrollValueChanged(Vector2 value)
        {
            var prevValue = _scrollValue;
            _scrollValue = value;
            // ScrollValueChangedListeners.Invoke(this, prevValue, value);
        }
    }
}
