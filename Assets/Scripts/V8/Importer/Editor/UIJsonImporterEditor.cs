using System;
using UnityEditor;

namespace V8
{
    public class UIJsonImporterEditor : Editor
    {
        private const string FILE_PANEL_TITLE = "Load UI Json";
        private const string JSON_EXTENSION = "json";


        [MenuItem("GameObject/UI/Json/Import")]
        private static void ImportAndBuild()
        {
            UIJsonImporter.ImportAndBuild(LoadJson(), true);
        }
        
        [MenuItem("GameObject/UI/Json/Release/Import(Set Release Form)")]
        private static void ImportAndBuildReleaseMode()
        {
            var result = EditorUtility.DisplayDialog("UI Json Import", "This is a \"Release Mode\" that allows you to check the UI layout when loading the UI JSON at runtime. In this state, you cannot export the UI JSON. Do you still want to load it?", "YES", "NO");
            if (result)
            {
                UIJsonImporter.ImportAndBuild(LoadJson(), false);    
            }
        }
        
        [MenuItem("GameObject/UI/Json/Clear")]
        private static void Clear()
        {
            UIJsonImporter.Release();
        }

        private static string LoadJson()
        {
            string path = EditorUtility.OpenFilePanel(FILE_PANEL_TITLE, "", JSON_EXTENSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("path cannot be an empty string", "path");
            }
            return path;
        }
    }
}