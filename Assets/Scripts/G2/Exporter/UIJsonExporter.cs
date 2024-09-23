using System;
using System.Collections.Generic;
using System.IO;
using G2.Importer;
using G2.Model;
using G2.Model.UI;
using G2.UI;
using G2.UI.Elements;
using Utilities;
using TMPro;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace G2.Exporter
{
    // todo: z-index 에 따른 GameObject 분할로 해당 구조에 맞게 Export 기능 수정 필요. 
    public class UIJsonExporter
    {
        private const string _END_POINT = "https://dinoksb.github.io/v8-new-test-ui-exchange";
        private const string _TEXTURE_RESOURCE_PATH = "StreamingAssets/Sprites";
        private const string _DIM_TYPE = "Dim";

        private static string DEV_END_POINT => Application.streamingAssetsPath;
        private const string DEV_TEXTURE_RESOURCE_PATH = "Sprites";

        public static void ExportWithRemotePath(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            UIData uiData = new();
            SetStudioData(gameObject, ref uiData.StudioData);
            SetSpriteData($"{_END_POINT}/{_TEXTURE_RESOURCE_PATH}", gameObject, ref uiData.Textures,
                ref uiData.SpriteSheets);
            SetUIData(gameObject, null, ref uiData.UI);
            SaveJson(filePath, uiData);
        }

        public static void ExportWithLocalPath(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            UIData uiData = new();
            SetStudioData(gameObject, ref uiData.StudioData);
            SetSpriteData($"{DEV_END_POINT}/{DEV_TEXTURE_RESOURCE_PATH}", gameObject, ref uiData.Textures,
                ref uiData.SpriteSheets);
            SetUIData(gameObject, null, ref uiData.UI);
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
            data.resolutionWidth = (uint)canvasScaler.referenceResolution.x;
            data.resolutionHeight = (uint)canvasScaler.referenceResolution.y;
        }

        private static void SetSpriteData(string textureFolderPath, GameObject gameObject,
            ref Dictionary<string, ResourceData> textures, ref Dictionary<string, SpriteSheetData> spriteSheets)
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
                    textures ??= new Dictionary<string, ResourceData>();
                    if (!textures.ContainsKey(texture.name))
                    {
                        textures.Add(texture.name, new ResourceData()
                        {
                            Name = texture.name,
                            Url = $"{textureFolderPath}/{texture.name}{Config.FileExtensions.PNG}",
                            // url = $"{END_POINT}/{TEXTURE_RESOURCE_PATH}/{texture.name}{UIConfig.PngExtension}",
                        });
                    }

                    spriteSheets ??= new Dictionary<string, SpriteSheetData>();
                    if (!spriteSheets.ContainsKey(sprite.name))
                    {
                        var webCoordinatePivot = GetSpritePivot(sprite).ToReverseYAxis();
                        spriteSheets.Add(sprite.name, new SpriteSheetData()
                        {
                            Name = sprite.name,
                            TextureId = texture.name,
                            CellSize = new[]
                                { Mathf.RoundToInt(sprite.rect.width), Mathf.RoundToInt(sprite.rect.height) },
                            Offset = new[] { Mathf.RoundToInt(sprite.rect.x), Mathf.RoundToInt(sprite.rect.y) },
                            Border = sprite.border.ToIntArray(),
                            Pivot = new[] { webCoordinatePivot.x, webCoordinatePivot.y },
                            PixelsPerUnit = sprite.pixelsPerUnit,
                            Multiple = GetSpriteMode(sprite) == SpriteImportMode.Multiple,
                        });
                    }
                }

                SetSpriteData(textureFolderPath, child.gameObject, ref textures, ref spriteSheets);
            }

            SpriteImportMode GetSpriteMode(Sprite sprite)
            {
                var spritePath = AssetDatabase.GetAssetPath(sprite);

                var importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
                if (importer == null)
                {
                    Debug.Log("[UIJsonExporter] Could not find the sprite sheet at the specified path.");
                    return SpriteImportMode.None;
                }

                return importer.spriteImportMode;
            }

            Vector2 GetSpritePivot(Sprite sprite)
            {
                var pivotPoint = sprite.pivot;
                var boundSize = sprite.bounds.size;
                var calcPivot = new Vector2(pivotPoint.x / boundSize.x / sprite.pixelsPerUnit,
                    pivotPoint.y / boundSize.y / sprite.pixelsPerUnit);

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
            ElementType? elementType = GetElementType(gameObject);
            if (elementType == null) return null;
            
            var target = gameObject.GetComponent<RectTransform>();

            switch (elementType)
            {
                case ElementType.Frame:
                    FrameData frameData = GetFrameData<FrameData>(target, guid);
                    var dimComponent = target.Find(_DIM_TYPE);
                    if (dimComponent)
                    {
                        var image = dimComponent.GetComponent<UnityEngine.UI.Image>();
                        frameData.dim = image.color.a;
                        frameData.interactable = image.raycastTarget;
                    }

                    return frameData as T;
                case ElementType.Image:
                    var imageBackgroundComponent = target.GetComponent<UnityEngine.UI.Image>();
                    if (imageBackgroundComponent.sprite != null)
                        return null;

                    var imageComponent = target.GetChild(0).GetComponent<UnityEngine.UI.Image>();
                    ImageData imageData = GetFrameData<ImageData>(target, guid);
                    var bgColor0To1 = imageBackgroundComponent.color.To01();
                    imageData.backgroundColor = new[]
                    {
                        bgColor0To1.r,
                        bgColor0To1.g,
                        bgColor0To1.b,
                        bgColor0To1.a
                    };

                    var imageColor0To1 = imageComponent.color.To01();
                    imageData.imageColor = new[]
                    {
                        imageColor0To1.r,
                        imageColor0To1.g,
                        imageColor0To1.b,
                        imageColor0To1.a
                    };
                    imageData.spriteId = imageComponent.sprite.name;
                    imageData.interactable = imageComponent.raycastTarget;
                    return imageData as T;
                case ElementType.Label:
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
                    labelData.interactable = textComponent.raycastTarget;
                    return labelData as T;
                case ElementType.Button:
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
                    InternalDebug.Log("[UIJsonExporter] - This type is not supported.");
                    return null;
            }


            T GetFrameData<T>(RectTransform target, string parent) where T : FrameData, new()
            {
                var webCoordinateAnchor = target.anchorMax.ToReverseYAxis();
                var webCoordinatePivot = target.pivot.ToReverseYAxis();
                var elementinfo = target.GetComponent<DevElementInfo>();
                T data = new T()
                {
                    name = target.name,
                    type = elementType.ToString(),
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
                    sizeConstraint = FrameData.ConstraintType.XY,
                    zIndex = elementinfo.ZIndex
                };
                return data;
            }
        }

        private static ElementType? GetElementType(GameObject gameObject)
        {
            // if (gameObject.name.Equals(UIConfig.DimType))
            // {
            //     typeName = UIConfig.DimType;
            //     return typeName;
            // }
            //
            // if (gameObject.name.Equals(UIConfig.ImageSource))
            // {
            //     return typeName;
            // }

            var imageComponent = gameObject.GetComponent<UnityEngine.UI.Image>();
            if (imageComponent != null)
            {
                if (imageComponent.transform.childCount != 0)
                    return ElementType.Image;
                
                return null;
            }


            var textComponent = gameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                return ElementType.Label;
            }

            var buttonComponent = gameObject.GetComponent<EventTrigger>();
            if (buttonComponent != null)
            {
                return ElementType.Button;
            }

            return ElementType.Frame;
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
