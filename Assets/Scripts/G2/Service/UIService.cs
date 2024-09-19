using System.Collections.Generic;
using G2.UI.Elements;
using UnityEngine;
using Utilities;

namespace G2.Service
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
            InternalDebug.Log($"[UIService] - {element.Name} OnFrontFrameVisibleChangedListener: " + isVisible);
        }
        
        private void OnVisibleChangedListener(IElement element)
        {
            var uid = element.Uid;
            if (!_visibleChangedElementsOrigin.ContainsKey(uid))
            {
                _visibleChangedElementsOrigin.Add(uid, element.Visible);
                _visibleChangedElementsReference.Add(uid, element);
                InternalDebug.Log($"[UIService] - {element.Name} OnVisibleChangedListener: " + element.Visible);
            }

            _isVisibleChanged = true;
        }

        private void OnPositionChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_positionChangedElementsOrigin.ContainsKey(uid))
            {
                _positionChangedElementsOrigin.Add(uid, new Vector2(element.Position.x, element.Position.y));
                _positionChangedElementsReference.Add(uid, element);
                InternalDebug.Log($"[UIService] - {element.Name} OnPositionChangedListener: " + element.Position);
            }

            _isPositionChange = true;
        }
        
        private void OnRotationChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_rotationChangedElementsOrigin.ContainsKey(uid))
            {
                _rotationChangedElementsOrigin.Add(uid, element.Rotation);
                _rotationChangedElementsReference.Add(uid, element);
                InternalDebug.Log($"[UIService] - {element.Name} OnRotationChangedListener: " + element.Rotation);
            }

            _isRotationChange = true;
        }
        
        private void OnSizeChangeListener(IElement element)
        {
            var uid = element.Uid;
            if (!_sizeChangedElementsOrigin.ContainsKey(uid))
            {
                _sizeChangedElementsOrigin.Add(uid, new Vector2(element.Size.x, element.Size.y));
                _sizeChangedElementsReference.Add(uid, element);
                InternalDebug.Log($"[UIService] - {element.Name} OnSizeChangedListener: " + element.Size);
            }

            _isSizeChange = true;
        }

        #endregion

        #region  Notify Events

        private void NotifyFrontFrameListener()
        {
            if (!_isFrontFrameVisibleChanged || _visibleChanagedElements.Count == 0) return;

            // notify listeners
            foreach (var frontFrameChangedEvent in _frontFrameChangedEvents)
            {
                frontFrameChangedEvent?.Invoke(_visibleChanagedElements.First.Value.Uid);
            }

            // flush
            _visibleChanagedElements.Clear();
            _isFrontFrameVisibleChanged = false;
        }
        
        private void NotifyVisibleChangedListener()
        {
            if (!_isVisibleChanged || _visibleChangedElementsOrigin.Count == 0 ||
                _visibleChangedElementsReference.Count == 0) return;

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

        private void NotifyPositionChangeListener()
        {
            if (!_isPositionChange || _positionChangedElementsOrigin.Count == 0 ||
                _positionChangedElementsReference.Count == 0) return;

            // notify listeners
            foreach (var elementValue in _positionChangedElementsReference)
            {
                var element = elementValue.Value;
                var uid = element.Uid;
                if (!CheckApproximately(element.Position, _positionChangedElementsOrigin[uid]))
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

        private void NotifyRotationChangeListener()
        {
            if (!_isRotationChange || _rotationChangedElementsOrigin.Count == 0 ||
                _rotationChangedElementsReference.Count == 0) return;

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
        
        private void NotifySizeChangeListener()
        {
            if (!_isSizeChange || _sizeChangedElementsOrigin.Count == 0 ||
                _sizeChangedElementsReference.Count == 0) return;

            // notify listeners
            foreach (var elementValue in _sizeChangedElementsReference)
            {
                var element = elementValue.Value;
                var uid = element.Uid;
                if (!CheckApproximately(element.Size, _sizeChangedElementsOrigin[uid]))
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
        
        // private void NotifyListeners<T>(
        //     ref bool isChanged, 
        //     Dictionary<string, T> originalElements, 
        //     Dictionary<string, IElement> referenceElements, 
        //     List<Action<string, T, T>> eventHandlers, 
        //     Func<IElement, T> getElementValue)
        // {
        //     if (!isChanged || originalElements.Count == 0 || referenceElements.Count == 0)
        //         return;
        //
        //     foreach (var referencePair in referenceElements)
        //     {
        //         var element = referencePair.Value;
        //         var uid = element.Uid;
        //
        //         // 원래 값과 새 값을 비교하여 다르면 이벤트 실행
        //         if (originalElements.TryGetValue(uid, out var originalValue))
        //         {
        //             var newValue = getElementValue(element);
        //             if (!EqualityComparer<T>.Default.Equals(newValue, originalValue))
        //             {
        //                 foreach (var handler in eventHandlers)
        //                 {
        //                     handler?.Invoke(uid, originalValue, newValue);
        //                 }
        //             }
        //         }
        //     }
        //
        //     // 상태 초기화
        //     originalElements.Clear();
        //     referenceElements.Clear();
        //     isChanged = false;
        // }
        
        #endregion

        private bool CheckApproximately(Vector2 v1, Vector2 v2)
        {
            if (Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y))
                return true;
            return false;
        }
    }
}