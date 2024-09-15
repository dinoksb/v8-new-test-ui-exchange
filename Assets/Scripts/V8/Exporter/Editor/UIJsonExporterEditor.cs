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
        private static void ExportWithLocalPath()
        {
            var path = EditorUtility.SaveFilePanel(FORDER_PANEL_TITLE, $"{Application.dataPath}/../", "", FILE_EXTIONSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("[UIJsonExporterEditor] - path cannot be an empty string", "path");
            }
            UIJsonExporter.ExportWithLocalPath(Selection.activeGameObject, path);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameObject/UI/Json/Release/Export(Set Remote Path)")]
        private static void ExportWithRemotePath()
        {
            var path = EditorUtility.SaveFilePanel(FORDER_PANEL_TITLE, $"{Application.dataPath}/../", "", FILE_EXTIONSION);
            if (path.Length == 0)
            {
                throw new ArgumentException("[UIJsonExporterEditor] - path cannot be an empty string", "path");
            }
            UIJsonExporter.ExportWithRemotePath(Selection.activeGameObject, path);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameObject/UI/Json/Export", true)]
        private static bool ExportValidation()
        {
            var canvas = Selection.activeGameObject?.GetComponent<UnityEngine.Canvas>();
            if (!canvas) return false;
            var editMode = canvas?.GetComponentInChildren<DevElementInfo>();
            if(!editMode) return false;

            return true;
        }
        
        [MenuItem("GameObject/UI/Json/Release/Export(Set Remote Path)", true)]
        private static bool DevelopmentExportValidation()
        {
            var canvas = Selection.activeGameObject?.GetComponent<UnityEngine.Canvas>();
            if (!canvas) return false;
            var editMode = canvas?.GetComponentInChildren<DevElementInfo>();
            if(!editMode) return false;

            return true;
        }
    }
}