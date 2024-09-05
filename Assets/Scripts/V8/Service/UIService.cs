using System;
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
            NotifyListeners(
                ref _isVisibleChanged,
                _visibleChangedElementsOrigin,
                _visibleChangedElementsReference,
                _visibleChangedEvents.ConvertAll(e => new Action<string, bool, bool>((uid, _, newVal) => e?.Invoke(uid, newVal))),
                element => element.Visible
            );
        }

        private void NotifyPositionChangeListener()
        {
            NotifyListeners(
                ref _isPositionChange,
                _positionChangedElementsOrigin,
                _positionChangedElementsReference,
                _positionChangedEvents.ConvertAll(e => new Action<string, Vector2, Vector2>((uid, oldVal, newVal) => e?.Invoke(uid, oldVal, newVal))),
                element => element.Position
            );
        }

        private void NotifyRotationChangeListener()
        {
            NotifyListeners(
                ref _isRotationChange,
                _rotationChangedElementsOrigin,
                _rotationChangedElementsReference,
                _rotationChangedEvents.ConvertAll(e => new Action<string, float, float>((uid, oldVal, newVal) => e?.Invoke(uid, oldVal, newVal))),
                element => element.Rotation
            );
        }
        
        private void NotifySizeChangeListener()
        {
            NotifyListeners(
                ref _isSizeChange,
                _sizeChangedElementsOrigin,
                _sizeChangedElementsReference,
                _sizeChangedEvents.ConvertAll(e => new Action<string, Vector2, Vector2>((uid, oldVal, newVal) => e?.Invoke(uid, oldVal, newVal))),
                element => element.Size
            );
        }
        
        private void NotifyListeners<T>(
            ref bool isChanged, 
            Dictionary<string, T> originalElements, 
            Dictionary<string, IElement> referenceElements, 
            List<Action<string, T, T>> eventHandlers, 
            Func<IElement, T> getElementValue)
        {
            if (!isChanged || originalElements.Count == 0 || referenceElements.Count == 0)
                return;

            foreach (var referencePair in referenceElements)
            {
                var element = referencePair.Value;
                var uid = element.Uid;

                // 원래 값과 새 값을 비교하여 다르면 이벤트 실행
                if (originalElements.TryGetValue(uid, out var originalValue))
                {
                    var newValue = getElementValue(element);
                    if (!EqualityComparer<T>.Default.Equals(newValue, originalValue))
                    {
                        foreach (var handler in eventHandlers)
                        {
                            handler?.Invoke(uid, originalValue, newValue);
                        }
                    }
                }
            }

            // 상태 초기화
            originalElements.Clear();
            referenceElements.Clear();
            isChanged = false;
        }
        
        #endregion
    }
}