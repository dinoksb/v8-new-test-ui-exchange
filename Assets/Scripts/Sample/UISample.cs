using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using V8;
using V8.Service;
using V8.Utilities;

public class UISample : MonoBehaviour
{
    public UnityEngine.Canvas UICanvas
    {
        get
        {
            if (!_uiCanvas)
            {
                var element = new V8.Canvas("UICanvas", null, new Vector2(Screen.width, Screen.height), false);
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

    private async void Start()
    {
        await LoadUIAsync(_url);
        Initialize();
        RegisterButtonEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var frontFrame = _uiManager.GetFrontFrame();
            InternalDebug.Log($"{frontFrame.Name} is front frame.");
        }
    }

    private async UniTask LoadUIAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            InternalDebug.LogError("url is null or empty");
            return;
        }
        
        var result = await WebRequestUtility.GetData(url);
        if (string.IsNullOrEmpty(result))
        {
            InternalDebug.LogError("result is null");
            return;
        }
        
        await _uiManager.LoadAsync(result);
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
    }
    
    private void RegisterButtonEvent()
    {
        // open z-index popup
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
        
        
        // open popup with dim
        _uiManager.MoveToFront(_popupWithDim);
        
        var openPopupWithDim = _uiManager.Get<Button>("c1acf2cf-4820-4546-b1a2-64e46d978c79");
        openPopupWithDim.AddPointerDownListener((element) =>
        {
            InternalDebug.Log($"{element.Name} pressed.");
            _popupWithDim.Visible = true;
            
            var frontFrame = _uiManager.GetFrontFrame();
            InternalDebug.Log($"{frontFrame.Name} is front frame.");
        });
        
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
}
