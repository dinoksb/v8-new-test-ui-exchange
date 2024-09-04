using UnityEngine;

namespace V8.Service
{
    public interface IUIService
    {
        public delegate void UidDelegate(string uid);
        public delegate void UidVisibilityChangedDelegate(string uid, bool isVisible);
        public delegate void UidFloatValueChangedDelegate(string uid, float prevValue, float newValue);
        public delegate void UidValueChangedDelegate(string uid, Vector2 prevValue, Vector2 newValue);
        
        public void OnCreated(IElement action);
        
        public void AddFrontFrameChangeListener(UidDelegate action);
        public void RemoveFrontFrameChangeListener(UidDelegate action);
        public void RemoveAllFrontFrameChangeListener();
        
        public void AddPositionChangedListener(UidValueChangedDelegate action);
        public void RemovePositionChangedListener(UidValueChangedDelegate action);
        public void RemoveAllPositionChangedListener();
        
        public void AddRotationChangedListener(UidFloatValueChangedDelegate action);
        public void RemoveRotationChangedListener(UidFloatValueChangedDelegate action);
        public void RemoveAllRotationChangedListener();

        public void AddSizeChangedListener(UidValueChangedDelegate action);
        public void RemoveSizeChangedListener(UidValueChangedDelegate action);
        public void RemoveAllSizeChangedListener();
        
        public void AddVisibleChangedListener(UidVisibilityChangedDelegate action);
        public void RemoveVisibleChangedListener(UidVisibilityChangedDelegate action);
        public void RemoveAllVisibleChangedListener();

        public void RemoveAllListener();
    }
}