using Cysharp.Threading.Tasks;
using UnityEngine;
using G2.Manager;
using G2.Service;
using G2.UI.Elements;
using G2.UI.Elements.Interactive;
using Utilities;

public class UISample : MonoBehaviour
{
    public UnityEngine.Canvas UICanvas
    {
        get
        {
            if (!_uiCanvas)
            {
                var element = new G2.UI.Elements.Basic.Canvas("UICanvas", null, new Vector2(Screen.width, Screen.height), false);
                _uiCanvas = element.Self.GetComponent<UnityEngine.Canvas>();
            }

            return _uiCanvas;
        }
    }
    
    [SerializeField] private string _url;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private UIService _uiService;
    
    private UnityEngine.Canvas _uiCanvas;

    private IElement _popupWithDim;
    private IElement _zIndexPopup1;
    private IElement _zIndexPopup2;
    
    private Rect _guiRect = new Rect(10, 10, 150, 100);

    private async void Start()
    {
        await LoadUIAsync(_url);
        Initialize();
        RegisterButtonEvent();
    }

    private async UniTask LoadUIAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            InternalDebug.LogError("url is null or empty");
            return;
        }
        
        var json = await WebRequestUtility.GetData(url);
        if (string.IsNullOrEmpty(json))
        {
            InternalDebug.LogError("result is null");
            return;
        }
        
        await _uiManager.LoadAsync("", json, default);
        _uiManager.Show(UICanvas);
    }

    private void Initialize()
    {
        _popupWithDim = _uiManager.Get("4f0ccb62-2ab6-4085-933b-c9016a53bdd0");
        _popupWithDim.Visible = false;
        
        _zIndexPopup1 = _uiManager.Get("27c72055-9d72-4cb6-9fac-a3cc957b52c9");
        _zIndexPopup1.Visible = false;

        _zIndexPopup2 = _uiManager.Get("0f32f82a-69b0-4e66-883e-59136fc82483");
        _zIndexPopup2.Visible = false;
        
        
        _uiService.AddPositionChangedListener((uid, prevValue, newValue) =>
        {
            InternalDebug.Log($"uid: {uid} - prevValue: {prevValue} - newValue: {newValue}");
        });
    }

    private void RegisterButtonEvent()
    {
        // open z-index popups button
        var openPopup = _uiManager.Get<Button>("dacdbc42-c781-4c7c-bd03-10b734c0c492");
        openPopup.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _zIndexPopup1.Visible = true;
            _zIndexPopup2.Visible = true;
        });
        
        // z-index popup 1
        var zIndexPopup1CloseButton = _uiManager.Get<Button>("6746f9a5-43f9-4591-a9f8-0d1166197aa8");
        zIndexPopup1CloseButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _zIndexPopup1.Visible = false;
        });
        
        var zIndexPopup1YesButton = _uiManager.Get<Button>("5ea0f20a-8a3f-486f-b19b-b79b6a7db188");
        zIndexPopup1YesButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _uiManager.MoveToFront(_zIndexPopup1);
            
            var frontFrame = _uiManager.GetFrontFrame();
            InternalDebug.Log($"{frontFrame.Name} is front frame.");
        });
        
        var zIndexPopup1NoButton = _uiManager.Get<Button>("d164bf0a-da7b-44af-af46-6f01e0d0d8bf");
        zIndexPopup1NoButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _zIndexPopup1.Visible = false;
        });
        
        // z-index popup2
        var zIndexPopup2CloseButton = _uiManager.Get<Button>("78d6760d-a3d1-4054-a573-897c1aa04c76");
        zIndexPopup2CloseButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _zIndexPopup2.Visible = false;
        });
        
        var zIndexPopup2YesButton = _uiManager.Get<Button>("a8d9536d-d533-482d-b755-30ad87a9d27c");
        zIndexPopup2YesButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _uiManager.MoveToFront(_zIndexPopup2);
            
            var frontFrame = _uiManager.GetFrontFrame();
            InternalDebug.Log($"{frontFrame.Name} is front frame.");
        });
        
        var zIndexPopup2NoButton = _uiManager.Get<Button>("7ae69e22-1224-4358-83ce-948fcd53b7e2");
        zIndexPopup2NoButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _zIndexPopup2.Visible = false;
        });
        
        // open popup with dim button
        _uiManager.MoveToFront(_popupWithDim);
        
        var openPopupWithDim = _uiManager.Get<Button>("c1acf2cf-4820-4546-b1a2-64e46d978c79");
        openPopupWithDim.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _popupWithDim.Visible = true;
            
            var frontFrame = _uiManager.GetFrontFrame();
            InternalDebug.Log($"{frontFrame.Name} is front frame.");
        });
        
        // dim popup
        var openPopupWithDimCloseButton = _uiManager.Get<Button>("a544e651-ba2b-49b0-bf99-ec96e553a162");
        openPopupWithDimCloseButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _popupWithDim.Visible = false;
        });
        
        var openPopupWithDimYesButton = _uiManager.Get<Button>("baddfc77-8598-40bf-9e29-fe36c5b0dc4c");
        openPopupWithDimYesButton.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _popupWithDim.Visible = false;
        });
        
    }

    private void MoveFrontElement(int pixels)
    {
        var frontFrame = _uiManager.GetFrontFrame();
        InternalDebug.Log($"{frontFrame.Name} is front frame.");
            
        frontFrame.Position = new Vector2(frontFrame.Position.x + pixels, frontFrame.Position.y);
    }
    
    private void RotationFrontElement(int degrees)
    {
        var frontFrame = _uiManager.GetFrontFrame();
        InternalDebug.Log($"{frontFrame.Name} is front frame.");
            
        frontFrame.Rotation += degrees;
    }
    
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(_guiRect, "Move Left (100 pixels)"))
        {
            MoveFrontElement(-100);
        }
        
        if (GUI.Button(new Rect(_guiRect.x + (_guiRect.width + 10), _guiRect.y, _guiRect.width, _guiRect.height), "Move Right (100 pixels)"))
        {
            MoveFrontElement(100);
        }
        
        if (GUI.Button(new Rect(_guiRect.x, _guiRect.y + (_guiRect.height + 10), _guiRect.width, _guiRect.height), "Rotation Left (90 degress)"))
        {
            RotationFrontElement(90);
        }
        
        if (GUI.Button(new Rect(_guiRect.x + (_guiRect.width + 10), _guiRect.y + (_guiRect.height + 10), _guiRect.width, _guiRect.height), "Rotation Right (90 degress)"))
        {
            RotationFrontElement(-90);
        }
        
        //a8d9536d-d533-482d-b755-30ad87a9d27c
        
        if (GUI.Button(new Rect(_guiRect.x + (_guiRect.width + 10), _guiRect.y + (_guiRect.height + 10) * 2, _guiRect.width, _guiRect.height), "Yes Button Move Front"))
        {
            var yesButton = _uiManager.Get<Button>("a8d9536d-d533-482d-b755-30ad87a9d27c");
            _uiManager.MoveToFront(yesButton);
        }
    }
#endif
}
