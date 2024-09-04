using System.Collections.Generic;
using UnityEngine;
using V8.Utilities;

namespace V8.Service
{
    public class UIService : MonoBehaviour, IUIService
    {
        private readonly LinkedList<IElement> _visibleChanagedElements = new();
        private readonly List<IUIService.UidDelegate> _frontFrameChangedEvents = new();

        private readonly Dictionary<string, Vector2> _positionChangedElementsOrigin = new();
        private readonly Dictionary<string, IElement> _positionChangedElementsReference = new();
        private readonly List<IUIService.UidValueChangedDelegate> _positionChangedEvents = new();

        private readonly Dictionary<string, float> _rotationChangedElementsOrigin = new();
        private readonly Dictionary<string, IElement> _rotationChangedElementsReference = new();
        private readonly List<IUIService.UidFloatValueChangedDelegate> _rotationChangedEvents = new();

        private readonly Dictionary<string, Vector2> _sizeChangedElementsOrigin = new();
        private readonly Dictionary<string, IElement> _sizeChangedElementsReference = new();
        private readonly List<IUIService.UidValueChangedDelegate> _sizeChangedEvents = new();
        
        private readonly Dictionary<string, bool> _visibleChangedElementsOrigin = new();
        private readonly Dictionary<string, IElement> _visibleChangedElementsReference = new();
        private readonly List<IUIService.UidVisibilityChangedDelegate> _visibleChangedEvents = new();

        private bool _isFrontFrameVisibleChanged = false;
        private bool _isVisibleChanged = false;
        private bool _isPositionChange = false;
        private bool _isRotationChange = false;
        private bool _isSizeChange = false;

        private void LateUpdate()
        {
            NotifyFrontFrameListener();
            NotifyVisibleChangedListener();
            NotifyPositionChangeListener();
            NotifyRotationChangeListener();
            NotifySizeChangeListener();
        }

        #region Interface implementation
        public void OnCreated(IElement element)
        {
            element.AddVisibleChangedListener(OnVisibleChangedForFrontFrameListener);
            element.AddVisibleChangedListener(OnVisibleChangedListener);
            element.AddPositionChangeListener(OnPositionChangeListener);
            element.AddRotationChangeListener(OnRotationChangeListener);
            element.AddSizeChangeListener(OnSizeChangeListener);
        }

        public void AddFrontFrameChangeListener(IUIService.UidDelegate action)
        {
            if (_frontFrameChangedEvents.Contains(action)) return;
            _frontFrameChangedEvents.Add(action);
        }

        public void RemoveFrontFrameChangeListener(IUIService.UidDelegate action)
        {
            _frontFrameChangedEvents.Remove(action);
        }

        public void RemoveAllFrontFrameChangeListener()
        {
            _frontFrameChangedEvents.Clear();
        }

        public void AddVisibleChangedListener(IUIService.UidVisibilityChangedDelegate action)
        {
            if (_visibleChangedEvents.Contains(action)) return;
            _visibleChangedEvents.Add(action);
        }

        public void RemoveVisibleChangedListener(IUIService.UidVisibilityChangedDelegate action)
        {
            _visibleChangedEvents.Remove(action);
        }

        public void RemoveAllVisibleChangedListener()
        {
            _visibleChangedEvents.Clear();
        }
        
        public void AddPositionChangedListener(IUIService.UidValueChangedDelegate action)
        {
            if (_positionChangedEvents.Contains(action)) return;
            _positionChangedEvents.Add(action);
        }

        public void RemovePositionChangedListener(IUIService.UidValueChangedDelegate action)
        {
            _positionChangedEvents.Remove(action);
        }

        public void RemoveAllPositionChangedListener()
        {
            _positionChangedEvents.Clear();
        }

        public void AddRotationChangedListener(IUIService.UidFloatValueChangedDelegate action)
        {
            if (_rotationChangedEvents.Contains(action)) return;
            _rotationChangedEvents.Add(action);
        }

        public void RemoveRotationChangedListener(IUIService.UidFloatValueChangedDelegate action)
        {
            _rotationChangedEvents.Remove(action);
        }

        public void RemoveAllRotationChangedListener()
        {
            _rotationChangedEvents.Clear();
        }

        public void AddSizeChangedListener(IUIService.UidValueChangedDelegate action)
        {
            if (_sizeChangedEvents.Contains(action)) return;
            _sizeChangedEvents.Add(action);
        }

        public void RemoveSizeChangedListener(IUIService.UidValueChangedDelegate action)
        {
            _sizeChangedEvents.Remove(action);
        }

        public void RemoveAllSizeChangedListener()
        {
            _sizeChangedEvents.Clear();
        }
        
        public void RemoveAllListener()
        {
            RemoveAllFrontFrameChangeListener();
            RemoveAllPositionChangedListener();
            RemoveAllRotationChangedListener();
            RemoveAllSizeChangedListener();
            RemoveAllVisibleChangedListener();
        }

        #endregion

        #region Interface Listener
        private void OnVisibleChangedForFrontFrameListener(IElement element)
        {
            bool isVisible = element.Visible;
            if (isVisible)
            {
                if (!_visibleChanagedElements.Contains(element))
                    _visibleChanagedElements.AddFirst(element);
            }
            else
            {
                _visibleChanagedElements.Remove(element);
            }

            _isFrontFrameVisibleChanged = true;
            InternalDebug.Log($"[UIService] - OnFrontFrameVisibleChangedListener: " + isVisible);
        }
        
        private void OnVisibleChangedListener(IElement element)
        {
            var uid = element.Uid;
            if (!_visibleChangedElementsOrigin.ContainsKey(uid))
            {
                _visibleChangedElementsOrigin.Add(uid, element.Visible);
                _visibleChangedElementsReference.Add(uid, element);
            }

            _isVisibleChanged = true;
            InternalDebug.Log($"[UIService] - OnVisibleChangedListener: " + element.Visible);
        }

        private void OnPositionChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_positionChangedElementsOrigin.ContainsKey(uid))
            {
                _positionChangedElementsOrigin.Add(uid, new Vector2(element.Position.x, element.Position.y));
                _positionChangedElementsReference.Add(uid, element);
            }

            _isPositionChange = true;
            InternalDebug.Log($"[UIService] - OnPositionChangedListener: " + element.Position);
        }
        
        private void OnRotationChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_rotationChangedElementsOrigin.ContainsKey(uid))
            {
                _rotationChangedElementsOrigin.Add(uid, element.Rotation);
                _rotationChangedElementsReference.Add(uid, element);
            }

            _isRotationChange = true;
            InternalDebug.Log($"[UIService] - OnRotationChangedListener: " + element.Rotation);
        }
        
        private void OnSizeChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_sizeChangedElementsOrigin.ContainsKey(uid))
            {
                _sizeChangedElementsOrigin.Add(uid, new Vector2(element.Size.x, element.Size.y));
                _sizeChangedElementsReference.Add(uid, element);
            }

            _isSizeChange = true;
            InternalDebug.Log($"[UIService] - OnSizeChangedListener: " + element.Size);
        }

        #endregion

        #region  Notify Events
        private void NotifyFrontFrameListener()
        {
            if (_isFrontFrameVisibleChanged)
            {
                if (_visibleChanagedElements.Count == 0) return;

                // notify listeners
                foreach (var frontFrameChangedEvent in _frontFrameChangedEvents)
                {
                    frontFrameChangedEvent?.Invoke(_visibleChanagedElements.First.Value.Uid);
                }

                // flush
                _visibleChanagedElements.Clear();
                _isFrontFrameVisibleChanged = false;
            }
        }
        
        private void NotifyVisibleChangedListener()
        {
            if (_isVisibleChanged)
            {
                if (_visibleChangedElementsOrigin.Count == 0 || _visibleChangedElementsReference.Count == 0) return;

                // notify listeners
                foreach (var elementValue in _visibleChangedElementsReference)
                {
                    var element = elementValue.Value;
                    var uid = element.Uid;
                    if (element.Visible != _visibleChangedElementsOrigin[uid])
                    {
                        foreach (var visibleChangedEvent in _visibleChangedEvents)
                        {
                            visibleChangedEvent?.Invoke(uid, element.Visible);
                        }
                    }
                }

                //flush
                _visibleChangedElementsOrigin.Clear();
                _visibleChangedElementsReference.Clear();
                _isVisibleChanged = false;
            }
        }

        private void NotifyPositionChangeListener()
        {
            if (_isPositionChange)
            {
                if (_positionChangedElementsOrigin.Count == 0 || _positionChangedElementsReference.Count == 0) return;

                // notify listeners
                foreach (var elementValue in _positionChangedElementsReference)
                {
                    var element = elementValue.Value;
                    var uid = element.Uid;
                    if (element.Position != _positionChangedElementsOrigin[uid])
                    {
                        foreach (var positionChangeEvent in _positionChangedEvents)
                        {
                            var prevValue = _positionChangedElementsOrigin[uid];
                            var newValue = element.Position;
                            positionChangeEvent?.Invoke(uid, prevValue, newValue);
                        }
                    }
                }

                //flush
                _positionChangedElementsOrigin.Clear();
                _positionChangedElementsReference.Clear();
                _isPositionChange = false;
            }
        }

        private void NotifyRotationChangeListener()
        {
            if (_isRotationChange)
            {
                if (_rotationChangedElementsOrigin.Count == 0 || _rotationChangedElementsReference.Count == 0) return;

                // notify listeners
                foreach (var elementValue in _rotationChangedElementsReference)
                {
                    var element = elementValue.Value;
                    var uid = element.Uid;
                    if (!Mathf.Approximately(element.Rotation, _rotationChangedElementsOrigin[uid]))
                    {
                        foreach (var rotationChangeEvent in _rotationChangedEvents)
                        {
                            var prevValue = _rotationChangedElementsOrigin[uid];
                            var newValue = element.Rotation;
                            rotationChangeEvent?.Invoke(uid, prevValue, newValue);
                        }
                    }
                }

                //flush
                _rotationChangedElementsOrigin.Clear();
                _rotationChangedElementsReference.Clear();
                _isRotationChange = false;
            }
        }
        
        private void NotifySizeChangeListener()
        {
            if (_isSizeChange)
            {
                if (_sizeChangedElementsOrigin.Count == 0 || _sizeChangedElementsReference.Count == 0) return;

                // notify listeners
                foreach (var elementValue in _sizeChangedElementsReference)
                {
                    var element = elementValue.Value;
                    var uid = element.Uid;
                    if (element.Size != _sizeChangedElementsOrigin[uid])
                    {
                        foreach (var sizeChangeEvent in _sizeChangedEvents)
                        {
                            var prevValue = _sizeChangedElementsOrigin[uid];
                            var newValue = element.Size;
                            sizeChangeEvent?.Invoke(uid, prevValue, newValue);
                        }
                    }
                }

                //flush
                _sizeChangedElementsOrigin.Clear();
                _sizeChangedElementsReference.Clear();
                _isSizeChange = false;
            }
        }
        #endregion
    }
}