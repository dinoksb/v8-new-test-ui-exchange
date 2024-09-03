using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Newtonsoft.Json;
using V8.Utilities;

namespace V8
{
    // todo: TextureData Export 할 수 있는 로직 추가.
    public class UIJsonExporter
    {
        private const string END_POINT = "https://dinoksb.github.io/v8-new-test-ui-exchange";
        private const string TEXTURE_RESOURCE_PATH = "StreamingAssets/Sprites";
        
        public static void Export(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            UIData uiData = new();
            SetStudioData(gameObject, ref uiData.studioData);
            SetSpriteData(gameObject, ref uiData.asset);
            SetUIData(gameObject, Guid.NewGuid().ToString(), ref uiData.ui);
            SaveJson(filePath, uiData);
        }

        private static void SaveJson(string filePath, UIData uiData)
        {
            string jsonString = JsonConvert.SerializeObject(uiData, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
            InternalDebug.Log($"ui json saved - {jsonString}");
        }

        private static void SetStudioData(GameObject gameObject, ref StudioData data)
        {
            var canvasScaler = gameObject.GetComponent<UnityEngine.UI.CanvasScaler>();
            data.version = Application.version;
            data.resolutionWidth = canvasScaler.referenceResolution.x;
            data.resolutionHeight = canvasScaler.referenceResolution.y;
        }

        private static void SetSpriteData(GameObject gameObject, ref AssetData data)
        {
            var childCount = gameObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                var imageComponent = child.GetComponent<UnityEngine.UI.Image>();
                if (imageComponent != null && imageComponent.sprite != null)
                {
                    var texture = imageComponent.mainTexture;
                    var sprite = imageComponent.sprite;
                    data.texture ??= new Dictionary<string, TextureData>();
                    if (!data.texture.ContainsKey(texture.name))
                    {
                        data.texture.Add(texture.name, new TextureData()
                        {
                            name = texture.name,
                            url = $"{END_POINT}/{TEXTURE_RESOURCE_PATH}/{texture.name}{UIConfig.PngExtension}",
                        });
                    }
                    
                    data.sprite ??= new Dictionary<string, SpriteData>();
                    data.sprite.Add(sprite.name, new SpriteData()
                    {
                        name = sprite.name,
                        textureId = texture.name,
                        size = new[] { sprite.rect.width, sprite.rect.height },
                        offset = new[] { sprite.rect.x, sprite.rect.y },
                        border = new[] { sprite.border.x, sprite.border.y, sprite.border.w, sprite.border.z },
                        pivot = new[] { sprite.pivot.x, sprite.pivot.y },
                        pixelsPerUnit = sprite.pixelsPerUnit
                    });
                }
                SetSpriteData(child.gameObject, ref data);
            }
        }

        private static void SetUIData(GameObject gameObject, string parentGUID, ref Dictionary<string, ElementData> data)
        {
            data ??= new Dictionary<string, ElementData>();
            parentGUID = string.IsNullOrEmpty(parentGUID) ? Guid.NewGuid().ToString() : parentGUID; 
            var childCount = gameObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                var guid = Guid.NewGuid().ToString();
                var element = GetElement<ElementData>(child.gameObject, parentGUID);
                if(element != null)
                    data.Add(guid, GetElement<ElementData>(child.gameObject, parentGUID));
                SetUIData(child.gameObject, guid, ref data);
            }
        }

        private static T GetElement<T>(GameObject gameObject, string guid) where T : ElementData
        {
            string typeName = GetTypeName(gameObject);

            var target = gameObject.GetComponent<RectTransform>();

            switch (typeName)
            {
                case UIConfig.FrameType:
                    FrameData frameData = GetFrameData<FrameData>(target, guid);
                    return frameData as T;
                case UIConfig.ImageType:
                    var imageComponent = target.GetComponent<UnityEngine.UI.Image>();
                    if (imageComponent.sprite == null) return null;
                    ImageData imageData = GetFrameData<ImageData>(target, guid);
                    imageData.spriteId = imageComponent.sprite.name;
                    imageData.imageColor = new[]
                    {
                        imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, imageComponent.color.a
                    };
                    return imageData as T;
                case UIConfig.LabelType:
                    var textComponent = target.GetComponent<TextMeshProUGUI>();
                    LabelData labelData = GetFrameData<LabelData>(target, guid);
                    labelData.textAlignment = textComponent.alignment.ToString();
                    labelData.fontId = textComponent.font.name;
                    labelData.fontColor = new[]
                        { textComponent.color.r, textComponent.color.g, textComponent.color.b, textComponent.color.a };
                    labelData.fontSize = textComponent.fontSize;
                    labelData.fontSizeConstraint = FrameData.ConstraintType.XX;
                    labelData.characterSpacing = textComponent.characterSpacing;
                    labelData.lineSpacing = textComponent.lineSpacing;
                    labelData.autoSize = true;
                    labelData.singleLine = textComponent.enableWordWrapping;
                    labelData.ellipsis = textComponent.overflowMode == TextOverflowModes.Ellipsis;
                    labelData.bold = (textComponent.fontStyle & FontStyles.Bold) == FontStyles.Bold;
                    labelData.italic = (textComponent.fontStyle & FontStyles.Italic) == FontStyles.Italic;
                    labelData.underline = (textComponent.fontStyle & FontStyles.Underline) == FontStyles.Underline;
                    labelData.strikethrough = (textComponent.fontStyle & FontStyles.Strikethrough) == FontStyles.Strikethrough;
                    labelData.text = textComponent.text;
                    return labelData as T;
                case UIConfig.ButtonType:
                    var eventTriggerComponent = target.GetComponent<EventTrigger>();
                    ButtonData buttonData = GetFrameData<ButtonData>(target, guid);
                    buttonData.events = new Dictionary<string, string>();
                    foreach (var trigger in eventTriggerComponent.triggers)
                    {
                        string eventId = trigger.eventID.ToString();
                        buttonData.events.Add(eventId, eventId);
                    }
                    // buttonData.threshold = 0.0f;
                    return buttonData as T;
                default:
                    InternalDebug.LogError("[UIJsonExporter] - This type is not supported.");
                    return null;
            }

            T GetFrameData<T>(RectTransform target, string parent) where T : FrameData, new()
            {
                T data = new T()
                {
                    name = target.name,
                    type = typeName,
                    parent = parent,
                    anchorMin = new List<float> { target.anchorMin.x, target.anchorMin.y },
                    anchorMax = new List<float> { target.anchorMax.x, target.anchorMax.y },
                    pivot = new List<float> { target.pivot.x, target.pivot.y },

                    position = new DimensionData()
                    {
                        x = new DimensionAdjustData
                        {
                            offset = target.anchoredPosition.x
                        },
                        y = new DimensionAdjustData
                        {
                            offset = target.anchoredPosition.y
                        }
                    },

                    size = new DimensionData()
                    {
                        x = new DimensionAdjustData
                        {
                            offset = target.sizeDelta.x
                        },
                        y = new DimensionAdjustData
                        {
                            offset = target.sizeDelta.y
                        }
                    },
                    rotation = target.rotation.eulerAngles.z,
                    visible = target.gameObject.activeSelf,
                    interactable = true,
                    sizeConstraint = FrameData.ConstraintType.XY
                };
                return data;
            }
        }

        private static string GetTypeName(GameObject gameObject)
        {
            string typeName = UIConfig.FrameType;

            var imageComponent = gameObject.GetComponent<UnityEngine.UI.Image>();
            if (imageComponent != null)
            {
                typeName = UIConfig.ImageType;
                return typeName;
            }

            var textComponent = gameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                typeName = UIConfig.LabelType;
                return typeName;
            }

            var buttonComponent = gameObject.GetComponent<EventTrigger>();
            if (buttonComponent != null)
            {
                typeName = UIConfig.ButtonType;
                return typeName;
            }

            return typeName;
        }

        private static bool IsValid(GameObject gameObject)
        {
            var canvas = gameObject.GetComponent<UnityEngine.Canvas>();
            if (canvas == null)
            {
                InternalDebug.LogError("It is not a hierarchical format that can be extracted as UI Json data.");
                return false;
            }

            return true;
        }
    }
}