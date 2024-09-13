using System;
using UnityEngine;
using V8;
using V8.Service;

public class UISample : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private UIService _uiService;
    
    private UnityEngine.Canvas _uiCanvas;

    private void Start()
    {
        
    }
    
    private async void LoadUIAsync(string uiUrl)
    {
        var json = await WebRequestUtility.GetData(uiUrl);
        if (string.IsNullOrEmpty(json))
            return;
        LoadUI(json);
    }
    
    private async void LoadUI(string jsonData)
    {
        await _uiManager.LoadAsync(jsonData);
        // _uiManager.Show(UICanvasForTest);
    }
}
