using UnityEngine;
using UnityEngine.Scripting;
using G2.Manager;
using G2.Service;
using UnityEngine.UI;
using Utilities;

public class Viewer : MonoBehaviour
{
    private const string _UI_CANVAS = "UICanvas";
    private const string UI_FRAME1_UID = "fc559ccd-ba30-4620-a3cf-6a35c3eb4df6";
    private const string UI_FRAME2_UID = "db57cf92-9fd8-440d-8650-4354103f17a3";
    private const string UI_FRAME3_UID = "ce1fc223-23c2-4a50-b589-f673dd322365";

    public Canvas UICanvas
    {
        get
        {
            if (!_uiCanvas)
            {
                _uiCanvas = CreateCanvas();
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

    public async void LoadUI(string json)
    {
        await _uiManager.LoadAsync("", json, true, default);

#if UNITY_WEBGL && !UNITY_EDITOR
            _uiManager.Show();
#endif
    }

    private async void LoadUIAsync(string uiUrl)
    {
        var json = await WebRequestUtility.GetData(uiUrl);
        if (string.IsNullOrEmpty(json))
        {
            InternalDebug.LogError("Error: UI Json is Null");
            return;
        }
        LoadUI(json);
    }

    [Preserve]
    public void ClearUI()
    {
        _uiManager.Release();
    }
    
    private Canvas CreateCanvas()
    {
        var gameObject = new GameObject(_UI_CANVAS);

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.vertexColorAlwaysGammaSpace = true;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

        var canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        canvasScaler.referencePixelsPerUnit = 100;
        canvasScaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    # region For Editor Test

    [ContextMenu("ShowCanvasTest")]
    private void ShowCanvasTest()
    {
        _uiManager.Show();
    }

    [ContextMenu("MoveToFrontA")]
    private void MoveToFrontA()
    {
        var element = _uiManager.Get(UI_FRAME1_UID);
        InternalDebug.Log("element: " + element.Name);
        _uiManager.MoveToFront(element);
    }

    // [ContextMenu("MoveToFrontB")]
    // private void MoveToFrontB()
    // {
    //     var element = _uiManager.Get(UI_FRAME2_UID);
    //     InternalDebug.Log("element: " + element.Name);
    //     _uiManager.MoveToFront(element);
    // }
    //
    // [ContextMenu("MoveToFrontC")]
    // private void MoveToFrontC()
    // {
    //     var element = _uiManager.Get(UI_FRAME3_UID);
    //     InternalDebug.Log("element: " + element.Name);
    //     _uiManager.MoveToFront(element);
    // }

    [ContextMenu("GetFrontFrame")]
    private void GetFrontFrame()
    {
        var element = _uiManager.GetFrontFrame();
        InternalDebug.Log("element: " + element.Name);
    }

    [ContextMenu("FrontFrameNotifyTest")]
    private void FrontFrameNotifyTest()
    {
        _uiService.AddFrontFrameChangeListener((uid) =>
            InternalDebug.Log($"[Viewer] - FrontFrameChanged: {_uiManager.Get(uid).Name}"));
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
        _uiService.AddVisibleChangedListener((uid, isVisible) =>
            InternalDebug.Log($"[Viewer] - OnVisibleChanged: {_uiManager.Get(uid).Name} / isVisible: {isVisible}"));

        var frame1 = _uiManager.Get(UI_FRAME1_UID);
        frame1.Visible = true;
        frame1.Visible = false;
        // frame1.Visible = true;
    }

    [ContextMenu("PositionChangeNotifyTest")]
    private void PositionChangeNotifyTest()
    {
        _uiService.AddPositionChangedListener((uid, prevValue, newValue) =>
            InternalDebug.Log(
                $"[Viewer] - OnPositionChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));

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
        _uiService.AddRotationChangedListener((uid, prevValue, newValue) =>
            InternalDebug.Log(
                $"[Viewer] - OnRotationChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));

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
        _uiService.AddSizeChangedListener((uid, prevValue, newValue) =>
            InternalDebug.Log(
                $"[Viewer] - OnSizeChange: {_uiManager.Get(uid).Name} / prevValue: {prevValue} / newValue: {newValue}"));

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
        if (GUI.Button(_guiRect, "Show"))
        {
            ShowCanvasTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + _guiRect.height + 10, _guiRect.width, _guiRect.height),
                "FrontFrame"))
        {
            FrontFrameNotifyTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 2, _guiRect.width, _guiRect.height),
                "Visible"))
        {
            VisibleChangedNotifyTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 3, _guiRect.width, _guiRect.height),
                "Position"))
        {
            PositionChangeNotifyTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 4, _guiRect.width, _guiRect.height),
                "Rotation"))
        {
            RotationChangeNotifyTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 5, _guiRect.width, _guiRect.height),
                "Size"))
        {
            SizeChangeNotifyTest();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 6, _guiRect.width, _guiRect.height),
                "Clear"))
        {
            ClearUI();
        }

        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10) * 7, _guiRect.width, _guiRect.height),
                "Reset"))
        {
            LoadUIAsync(_url);
        }

        if (GUI.Button(new Rect(_guiRect.x + _guiRect.width + 10, _guiRect.y, _guiRect.width, _guiRect.height),
                "MoveToFrontA"))
        {
            MoveToFrontA();
        }

        if (GUI.Button(
                new Rect(_guiRect.x + _guiRect.width + 10, _guiRect.y + _guiRect.height + 10, _guiRect.width,
                    _guiRect.height), "GetFrontFrame"))
        {
            GetFrontFrame();
        }
    }
#endif

    #endregion
}
