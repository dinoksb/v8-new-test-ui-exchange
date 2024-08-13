using System;
using UnityEditor;

namespace V8
{
    public class UIJsonExporterEditor : Editor
    {
        private const string FORDER_PANEL_TITLE = "Load UI Json";
        
        [MenuItem("GameObject/UI/Json/Export")]
        private static void Export()
        {
            var path = EditorUtility.OpenFolderPanel(FORDER_PANEL_TITLE, "", "");
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