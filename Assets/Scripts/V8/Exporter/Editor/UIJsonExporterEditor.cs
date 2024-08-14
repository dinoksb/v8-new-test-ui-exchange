using System;
using UnityEngine;
using UnityEditor;

namespace V8
{
    public class UIJsonExporterEditor : Editor
    {
        private const string FORDER_PANEL_TITLE = "Save UI Json";
        private const string FILE_EXTENSION = "json";
        
        [MenuItem("GameObject/UI/Json/Export")]
        private static void Export()
        {
            var path = EditorUtility.SaveFilePanel(FORDER_PANEL_TITLE, $"{Application.dataPath}/../", "", FILE_EXTENSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("[UIJsonExporterEditor] - path cannot be an empty string", "path");
            }
            UIJsonExporter.Export(Selection.activeGameObject, path);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameObject/UI/Json/Export", true)]
        private static bool ExportValidation()
        {
            var canvas = Selection.activeGameObject?.GetComponent<UnityEngine.Canvas>();
            return canvas;
        }
    }
}