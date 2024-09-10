using System;
using UnityEditor;
using UnityEngine;

namespace V8
{
    public class UIJsonExporterEditor : Editor
    {
        private const string FORDER_PANEL_TITLE = "Load UI Json";
        private const string FILE_EXTIONSION = "json";
        
        [MenuItem("GameObject/UI/Json/Export")]
        private static void Export()
        {
            var path = EditorUtility.SaveFilePanel(FORDER_PANEL_TITLE, $"{Application.dataPath}/../", "", FILE_EXTIONSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("[UIJsonExporterEditor] - path cannot be an empty string", "path");
            }
            UIJsonExporter.Export(Selection.activeGameObject, path);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameObject/UI/Json/Development_Export")]
        private static void DevelopmentExport()
        {
            var path = EditorUtility.SaveFilePanel(FORDER_PANEL_TITLE, $"{Application.dataPath}/../", "", FILE_EXTIONSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("[UIJsonExporterEditor] - path cannot be an empty string", "path");
            }
            UIJsonExporter.DevelopmentExport(Selection.activeGameObject, path);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameObject/UI/Json/Export", true)]
        private static bool ExportValidation()
        {
            var canvas = Selection.activeGameObject?.GetComponent<UnityEngine.Canvas>();
            return canvas;
        }
        
        [MenuItem("GameObject/UI/Json/Development_Export", true)]
        private static bool DevelopmentExportValidation()
        {
            var canvas = Selection.activeGameObject?.GetComponent<UnityEngine.Canvas>();
            return canvas;
        }
    }
}