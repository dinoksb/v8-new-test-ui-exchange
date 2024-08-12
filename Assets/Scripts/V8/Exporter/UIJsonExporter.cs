using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.EventSystems;
using V8.Utilities;

namespace V8
{
    public class UIJsonExporter
    {
        private const string FILE_NAME = "ui";
        private Dictionary<string, ElementData> _ui = new();

        public static void Export(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;

            int rootChildCount = gameObject.transform.childCount;

            for (int i = 0; i < rootChildCount; ++i)
            {
                var rootChild = gameObject.transform.GetChild(i);

                var typeName = GetTypeName(rootChild.gameObject);
                var type = GetElementFormTypeName<ElementData>(typeName);

                InternalDebug.Log("type: " + type);
            }

            InternalDebug.Log($"rootChildCount: {rootChildCount}");

            SaveJson(filePath);
        }

        private static void SaveJson(string filePath)
        {
            StudioData studioData;
            studioData.version = Application.version;
            studioData.resolutionWidth = Screen.width;
            studioData.resolutionHeight = Screen.height;

            UIData uiData = new();
            uiData.studioData = studioData;

            string jsonString = JsonConvert.SerializeObject(uiData, Formatting.Indented);
            File.WriteAllText($"{Path.Combine(filePath, FILE_NAME)}.json", jsonString);
            InternalDebug.Log($"ui json saved - {jsonString}");
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

        private static T GetElementFormTypeName<T>(string name) where T : ElementData
        {
            // TODO: 타입은 제대로 가져옴. 여기서 값을 넣어야하나??
            switch (name)
            {
                case "Frame":
                    return new FrameData() as T;
                case "Image":
                    return new ImageData() as T;
                case "Label":
                    return new LabelData() as T;
                case "Button":
                    return new ButtonData() as T;
                default:
                    return null;
            }

            return null;
        }
    }
}