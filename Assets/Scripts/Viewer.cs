using UnityEngine;
using UnityEngine.Scripting;
using V8.Service;
using V8.Utilities;

namespace V8.Template
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] private string _url;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private UIService _uiService;

        private void Start()
        {
#if UNITY_EDITOR
            LoadUIAsync(_url);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif
        }

        private void OnDestroy()
        {
            _uiService.RemoveAllListener();
        }

        public void LoadUI(string jsonData)
        {
            _uiManager.Load(jsonData);
        }

        [Preserve]
        public void ClearUI()
        {
            _uiManager.Release();
        }
        
        private async void LoadUIAsync(string uiUrl)
        {
            var json = await WebRequestUtility.GetData(uiUrl);
            if (string.IsNullOrEmpty(json)) return;
            LoadUI(json);
        }

        [ContextMenu("GetUIDTest")]
        private void Test()
        {
            var elementButton = _uiManager.Get<Image>("a80da9c9-2ec6-4751-a514-617680280177");
            Debug.Log("elementButton.Uid: " + elementButton.Uid);
        }

        [ContextMenu("FrontFrameNotifyTest")]
        private void FrontFrameNotifyTest()
        {
            _uiService.AddFrontFrameChangeListener((uid) => InternalDebug.Log($"[Viewer] - FrontFrameChanged: {_uiManager.Get(uid).Name}"));
            var frame1 = _uiManager.Get("96f2ee3a-37ae-442c-8f44-05e08a1e10eb");
            frame1.Visible = true;

            var label1 = _uiManager.Get("01ce13f0-1a89-4845-bc0c-922de85bb471");
            label1.Visible = true;
            
            var label2 = _uiManager.Get("5311749d-70e7-4aae-832f-f6e258c02e3b");
            label2.Visible = true;

            label2.Visible = false;
            // label1.Visible = false;
            // frame1.Visible = false;
        }
        
        [ContextMenu("VisibleChangedNotifyTest")]
        private void VisibleChangedNotifyTest()
        {
            _uiService.AddVisibleChangedListener((uid, isVisible) => InternalDebug.Log($"[Viewer] - OnVisibleChanged: {_uiManager.Get(uid).Name} / isVisible: {isVisible}"));
            
            var frame1 = _uiManager.Get("96f2ee3a-37ae-442c-8f44-05e08a1e10eb");
            frame1.Visible = true;
            frame1.Visible = false;
            // frame1.Visible = true;
        }

        [ContextMenu("PositionChangeNotifyTest")]
        private void PositionChangeNotifyTest()
        {
            _uiService.AddPositionChangedListener((uid, prevValue, newValue) => InternalDebug.Log($"[Viewer] - OnPositionChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));
            
            var frame1 = _uiManager.Get("96f2ee3a-37ae-442c-8f44-05e08a1e10eb");
            frame1.Position = new Vector2(0, 0);
            frame1.Position = new Vector2(1, 0);
            frame1.Position = new Vector2(2, 0);
            frame1.Position = new Vector2(3, 0);
            // frame1.Position = new Vector2(0, 0);
        }
        
        [ContextMenu("RotationChangeNotifyTest")]
        private void RotationChangeNotifyTest()
        {
            _uiService.AddRotationChangedListener((uid, prevValue, newValue) => InternalDebug.Log($"[Viewer] - OnRotationChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));
            
            var frame1 = _uiManager.Get("96f2ee3a-37ae-442c-8f44-05e08a1e10eb");
            frame1.Rotation = 50;
            frame1.Rotation = 60;
            frame1.Rotation = 80;
            frame1.Rotation = 90;
            // frame1.Position = 50;
        }
        
        [ContextMenu("SizeChangeNotifyTest")]
        private void SizeChangeNotifyTest()
        {
            _uiService.AddSizeChangedListener((uid, prevValue, newValue) => InternalDebug.Log($"[Viewer] - OnSizeChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));
            
            var frame1 = _uiManager.Get("96f2ee3a-37ae-442c-8f44-05e08a1e10eb");
            frame1.Size = new Vector2(100, 100);
            frame1.Size = new Vector2(200, 200);
            frame1.Size = new Vector2(300, 300);
            frame1.Size = new Vector2(500, 500);
            // frame1.Size = new Vector2(100, 100);
        }
    }
}
