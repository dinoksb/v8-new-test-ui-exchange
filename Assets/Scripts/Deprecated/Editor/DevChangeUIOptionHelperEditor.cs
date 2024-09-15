using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DevChangeUIOptionHelper))]
public class DevChangeUIOptionHelperEditor : Editor
{
    private static DevChangeUIOptionHelper _instance;

    private void OnEnable()
    {
        _instance ??= (DevChangeUIOptionHelper)target;
    }

    private const float _ratio = 0.75f;
    

    [MenuItem("GameObject/UI/SetSize And Position By Ratio")]
    private static void SetSizeAndPositionByRatio()
    {
        var gameObject = Selection.activeGameObject;
        SetSize(gameObject, _instance.Ratio);
        SetPosition(gameObject, _instance.Ratio);
        SetFontSize(gameObject, _instance.Ratio);
    }
    
    [MenuItem("GameObject/UI/Set Z-Index With Children")]
    private static void SetZIndexWithChildren()
    {
        var gameObject = Selection.activeGameObject;
        SetZIndex(gameObject);
    }
    
    
    private static void SetSize(GameObject gameObject, float ratio)
    {
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * ratio, rectTransform.sizeDelta.y * ratio);

        var childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            SetSize(child.gameObject, ratio);
        }
    }
    
    private static void SetPosition(GameObject gameObject, float ratio)
    {
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x * ratio, rectTransform.anchoredPosition.y * ratio);

        var childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            SetPosition(child.gameObject, ratio);
        }
    }
    
    private static void SetFontSize(GameObject gameObject, float ratio)
    {
        var textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        
        if(textMeshPro != null)
            textMeshPro.fontSize *= ratio;

        var childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            SetFontSize(child.gameObject, ratio);
        }
    }
    
    private static void SetZIndex(GameObject gameObject)
    {
        var elementInfo = gameObject.GetComponent<DevElementInfo>();
        if(elementInfo)
            elementInfo.ZIndex = _instance.ZIndex;
        
        var childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            SetZIndex(child.gameObject);
        }
    }
}
