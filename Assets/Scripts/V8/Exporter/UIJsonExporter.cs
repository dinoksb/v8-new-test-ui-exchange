using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using V8.Utilities;

namespace V8
{
    public class UIJsonExporter
    {
        private const string FILE_NAME = "ui";
        public static void Export(GameObject gameObject, string filePath)
        {
            if (!IsValid(gameObject)) return;
            
            int childCount = gameObject.transform.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                var child = gameObject.transform.GetChild(i);
                
            }
            
            InternalDebug.Log($"childCount: {childCount}");

            SaveJson(filePath);
        }

        private static void SaveJson(string filePath)
        {
            StudioData studioData;
            studioData.version = Application.version;
            studioData.resolutionWidth = Screen.width;
            studioData.resolutionHeight = Screen.height;
            
            UIData uiData = new ();
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
    }
}