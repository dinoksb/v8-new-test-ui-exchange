using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UISizeChanger : Editor
{
    private const float _ratio = 0.75f;

    [MenuItem("GameObject/UI/SetSizeAndPositionToHalfRatio")]
    private static void SetSizeAndPositionByRatio()
    {
        var gameObject = Selection.activeGameObject;
        SetSize(gameObject, _ratio);
        SetPosition(gameObject, _ratio);
        SetFontSize(gameObject, _ratio);
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
}
