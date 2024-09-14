using System;
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

    private void Start()
    {
        LoadUIAsync(_url);
    }
    
    private async void LoadUIAsync(string url)
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
}
