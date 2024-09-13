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
    // todo: z-index 에 따른 GameObject 분할로 해당 구조에 맞게 Export 기능 수정 필요. 
    public class UIJsonExporter
    {
        private const string END_POINT = "https://dinoksb.github.io/v8-new-test-ui-exchange";
        private const string TEXTURE_RESOURCE_PATH = "StreamingAssets/Sprites";

        private static string DEV_END_POINT => Application.streamingAssetsPath;
        private const string DEV_TEXTURE_RESOURCE_PATH = "Sprites";
        
        public static void Export(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            UIData uiData = new();
            SetStudioData(gameObject, ref uiData.studioData);
            SetSpriteData($"{END_POINT}/{TEXTURE_RESOURCE_PATH}", gameObject, ref uiData.asset);
            SetUIData(gameObject, null, ref uiData.ui);
            SaveJson(filePath, uiData);
        }
        
        public static void DevelopmentExport(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            UIData uiData = new();
            SetStudioData(gameObject, ref uiData.studioData);
            SetSpriteData($"{DEV_END_POINT}/{DEV_TEXTURE_RESOURCE_PATH}", gameObject, ref uiData.asset);
            SetUIData(gameObject, null, ref uiData.ui);
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

        private static void SetSpriteData(string textureFolderPath, GameObject gameObject, ref AssetData data)
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
                    data.resource ??= new Dictionary<string, ResourceData>();
                    if (!data.resource.ContainsKey(texture.name))
                    {
                        data.resource.Add(texture.name, new ResourceData()
                        {
                            name = texture.name,
                            url = $"{textureFolderPath}/{texture.name}{UIConfig.PngExtension}",
                            // url = $"{END_POINT}/{TEXTURE_RESOURCE_PATH}/{texture.name}{UIConfig.PngExtension}",
                        });
                    }

                    data.sprite ??= new Dictionary<string, SpriteData>();
                    if (!data.sprite.ContainsKey(sprite.name))
                    {
                        var webCoordinatePivot = GetSpritePivot(sprite).ToReverseYAxis();
                        data.sprite.Add(sprite.name, new SpriteData()
                        {
                            name = sprite.name,
                            textureId = texture.name,
                            size = new[] { sprite.rect.width, sprite.rect.height },
                            offset = new[] { sprite.rect.x, sprite.rect.y },
                            border = new[] { sprite.border.x, sprite.border.y, sprite.border.z, sprite.border.w },
                            pivot = new[] { webCoordinatePivot.x, webCoordinatePivot.y},
                            pixelsPerUnit = sprite.pixelsPerUnit
                        });
                    }
                }

                SetSpriteData(textureFolderPath, child.gameObject, ref data);
            }
            
            Vector2 GetSpritePivot(Sprite sprite)
            {
                var pivotPoint = sprite.pivot;
                var boundSize = sprite.bounds.size;
                var calcPivot = new Vector2(pivotPoint.x / boundSize.x / sprite.pixelsPerUnit, pivotPoint.y / boundSize.y / sprite.pixelsPerUnit);

                return new Vector2(calcPivot.x, calcPivot.y);
            }
        }

        private static void SetUIData(GameObject gameObject, string parentUID,
            ref Dictionary<string, ElementData> data)
        {
            data ??= new Dictionary<string, ElementData>();
            // parentGUID = string.IsNullOrEmpty(parentGUID) ? Guid.NewGuid().ToString() : parentGUID;
            var childCount = gameObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                var guid = Guid.NewGuid().ToString();
                var element = GetElement<ElementData>(child.gameObject, parentUID);

                if (element == null) continue;

                data.Add(guid, element);
                SetUIData(child.gameObject, guid, ref data);
            }
        }

        private static T GetElement<T>(GameObject gameObject, string guid) where T : ElementData
        {
            string typeName = GetTypeName(gameObject);

            if (string.IsNullOrEmpty(typeName)) return null;

            var target = gameObject.GetComponent<RectTransform>();

            switch (typeName)
            {
                case UIConfig.FrameType:
                    FrameData frameData = GetFrameData<FrameData>(target, guid);
                    var dimComponent = target.Find(UIConfig.DimType);
                    if (dimComponent)
                    {
                        var image = dimComponent.GetComponent<UnityEngine.UI.Image>();
                        frameData.dim = image.color.a;
                    }
                    return frameData as T;
                case UIConfig.ImageType:
                    var imageBackgroundComponent = target.GetComponent<UnityEngine.UI.Image>();
                    if (imageBackgroundComponent.sprite != null)
                        return null;

                    var imageComponent = target.GetChild(0).GetComponent<UnityEngine.UI.Image>();
                    ImageData imageData = GetFrameData<ImageData>(target, guid);
                    var bgColor0To1 = TypeConverter.ToColor0To1(new float[]
                    {
                        imageBackgroundComponent.color.r,
                        imageBackgroundComponent.color.g,
                        imageBackgroundComponent.color.b,
                        imageBackgroundComponent.color.a
                    });
                    imageData.backgroundColor = new[]
                    {
                        bgColor0To1.r,
                        bgColor0To1.g,
                        bgColor0To1.b,
                        bgColor0To1.a
                    };

                    var imageColor0To1 = TypeConverter.ToColor0To1(new float[]
                    {
                        imageComponent.color.r,
                        imageComponent.color.g,
                        imageComponent.color.b,
                        imageComponent.color.a
                    });
                    imageData.imageColor = new[]
                    {
                        imageColor0To1.r,
                        imageColor0To1.g,
                        imageColor0To1.b,
                        imageColor0To1.a
                    };
                    imageData.spriteId = imageComponent.sprite.name;
                    return imageData as T;
                case UIConfig.LabelType:
                    var textComponent = target.GetComponent<TextMeshProUGUI>();
                    LabelData labelData = GetFrameData<LabelData>(target, guid);
                    labelData.textAlignment = textComponent.alignment.ToString();
                    labelData.fontId = textComponent.font.name;
                    labelData.fontColor = new[]
                        { textComponent.color.r, textComponent.color.g, textComponent.color.b, textComponent.color.a };
                    labelData.fontSize = textComponent.fontSize;
                    // Todo: 추후 추가 될 수 있는 데이터.
                    // labelData.fontSizeConstraint = FrameData.ConstraintType.XX;
                    // labelData.characterSpacing = textComponent.characterSpacing;
                    // labelData.lineSpacing = textComponent.lineSpacing;
                    // labelData.autoSize = true;
                    // labelData.singleLine = textComponent.enableWordWrapping;
                    // labelData.ellipsis = textComponent.overflowMode == TextOverflowModes.Ellipsis;
                    // labelData.bold = (textComponent.fontStyle & FontStyles.Bold) == FontStyles.Bold;
                    // labelData.italic = (textComponent.fontStyle & FontStyles.Italic) == FontStyles.Italic;
                    // labelData.underline = (textComponent.fontStyle & FontStyles.Underline) == FontStyles.Underline;
                    // labelData.strikethrough =
                    //     (textComponent.fontStyle & FontStyles.Strikethrough) == FontStyles.Strikethrough;
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
                case UIConfig.DimType:
                    InternalDebug.Log($"[UIJsonExporter] - {UIConfig.DimType} type is do nothing.");
                    return null;
                default:
                    InternalDebug.LogError("[UIJsonExporter] - This type is not supported.");
                    return null;
            }


            T GetFrameData<T>(RectTransform target, string parent) where T : FrameData, new()
            {
                var webCoordinateAnchor = target.anchorMax.ToReverseYAxis();
                var webCoordinatePivot = target.pivot.ToReverseYAxis();
                
                T data = new T()
                {
                    name = target.name,
                    type = typeName,
                    parent = parent,
                    anchor = new[] { webCoordinateAnchor.x, webCoordinateAnchor.y },
                    pivot = new[] { webCoordinatePivot.x, webCoordinatePivot.y },

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
            string typeName = string.Empty;

            if (gameObject.name.Equals(UIConfig.DimType))
            {
                typeName = UIConfig.DimType;
                return typeName;
            }

            if (gameObject.name.Equals(UIConfig.ImageSource))
            {
                return typeName;
            }

            var imageComponent = gameObject.GetComponent<UnityEngine.UI.Image>();
            if (imageComponent != null && imageComponent.transform.childCount != 0)
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

            typeName = UIConfig.FrameType;
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

        private static float[] ConvertToWebCoordinateArray(Vector2 vector2)
        {
            return new[] { vector2.x, 1 - vector2.y };
        }
    }
}