using UnityEngine;
using UnityEngine.Scripting;
using V8.Service;
using V8.Utilities;

namespace V8.Template
{
    public class Viewer : MonoBehaviour
    {
        private const string UI_FRAME1_UID = "8600305e-ee85-436b-8c61-5562a019de0a";
        private const string UI_FRAME2_UID = "6c60a37c-6f23-46f7-8dd4-e21e2a2ef3c8";
        private const string UI_FRAME3_UID = "10aef319-ed5e-4cbf-8988-7912960ffde4";
        
        public UnityEngine.Canvas UICanvas
        {
            get
            {
                if (!_uiCanvas)
                {
                    var element = new Canvas("UICanvas", null, new Vector2(Screen.width, Screen.height), false);
                    _uiCanvas = element.Self.GetComponent<UnityEngine.Canvas>();
                }

                return _uiCanvas;
            }
        }
        
        [SerializeField] private string _url;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private UIService _uiService;
        
        private UnityEngine.Canvas _uiCanvas;

        private Rect _guiRect = new Rect(10, 10, 100, 50);
        
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

        public async void LoadUI(string jsonData)
        {
            await _uiManager.LoadAsync(jsonData);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            _uiManager.Show(UICanvasForTest);
#endif
        }
        
        private async void LoadUIAsync(string uiUrl)
        {
            var json = await WebRequestUtility.GetData(uiUrl);
            if (string.IsNullOrEmpty(json))
            {
                InternalDebug.LogError("Error: UI Json is Null");
                return;
            };
            LoadUI(json);
        }

        [Preserve]
        public void ClearUI()
        {
            _uiManager.Release();
        }

        # region For Editor Test
        [ContextMenu("ShowCanvasTest")]
        private void ShowCanvasTest()
        {
            _uiManager.Show(UICanvas);
        }

        [ContextMenu("FrontFrameNotifyTest")]
        private void FrontFrameNotifyTest()
        {
            _uiService.AddFrontFrameChangeListener((uid) => InternalDebug.Log($"[Viewer] - FrontFrameChanged: {_uiManager.Get(uid).Name}"));
            var frame1 = _uiManager.Get(UI_FRAME1_UID);
            frame1.Visible = true;
            frame1.Visible = false;

            var frame2 = _uiManager.Get(UI_FRAME2_UID);
            frame2.Visible = true;
            frame2.Visible = false;
            
            var frame3 = _uiManager.Get(UI_FRAME3_UID);
            frame3.Visible = true;
            frame3.Visible = false;

            frame2.Visible = true;
            // label1.Visible = false;
            // frame1.Visible = false;
        }
        
        [ContextMenu("VisibleChangedNotifyTest")]
        private void VisibleChangedNotifyTest()
        {
            _uiService.AddVisibleChangedListener((uid, isVisible) => InternalDebug.Log($"[Viewer] - OnVisibleChanged: {_uiManager.Get(uid).Name} / isVisible: {isVisible}"));
            
            var frame1 = _uiManager.Get(UI_FRAME1_UID);
            frame1.Visible = true;
            frame1.Visible = false;
            // frame1.Visible = true;
        }

        [ContextMenu("PositionChangeNotifyTest")]
        private void PositionChangeNotifyTest()
        {
            _uiService.AddPositionChangedListener((uid, prevValue, newValue) => InternalDebug.Log($"[Viewer] - OnPositionChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));
            
            var frame1 = _uiManager.Get(UI_FRAME1_UID);
            frame1.Position = new Vector2(0, 0);
            frame1.Position = new Vector2(100, 0);
            frame1.Position = new Vector2(200, 0);
            frame1.Position = new Vector2(300, 0);
            // frame1.Position = new Vector2(0, 0);
        }
        
        [ContextMenu("RotationChangeNotifyTest")]
        private void RotationChangeNotifyTest()
        {
            _uiService.AddRotationChangedListener((uid, prevValue, newValue) => InternalDebug.Log($"[Viewer] - OnRotationChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));
            
            var frame1 = _uiManager.Get(UI_FRAME1_UID);
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
            
            var frame1 = _uiManager.Get(UI_FRAME1_UID);
            frame1.Size = new Vector2(100, 100);
            frame1.Size = new Vector2(200, 200);
            frame1.Size = new Vector2(300, 300);
            frame1.Size = new Vector2(500, 500);
            // frame1.Size = new Vector2(100, 100);
        }

        #if UNITY_EDITOR
        private void OnGUI()
        {
            if(GUI.Button(_guiRect, "Show"))
            {
                ShowCanvasTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + _guiRect.height + 10, _guiRect.width, _guiRect.height), "FrontFrame"))
            {
                FrontFrameNotifyTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 2 , _guiRect.width, _guiRect.height), "Visible"))
            {
                VisibleChangedNotifyTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 3, _guiRect.width, _guiRect.height), "Position"))
            {
                PositionChangeNotifyTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 4, _guiRect.width, _guiRect.height), "Rotation"))
            {
                RotationChangeNotifyTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 5, _guiRect.width, _guiRect.height), "Size"))
            {
                SizeChangeNotifyTest();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 6, _guiRect.width, _guiRect.height), "Clear"))
            {
                ClearUI();
            }
            
            if(GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 7, _guiRect.width, _guiRect.height), "Reset"))
            {
                LoadUIAsync(_url);
            }
        }
        #endif
        #endregion
    }
}
