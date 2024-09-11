using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using V8.Utilities;

namespace V8
{
    public class UIJsonImporterEditor : Editor
    {
        private const string FILE_PANEL_TITLE = "Load UI Json";
        private const string JSON_EXTENSION = "json";

        private static void Import()
        {
            UIJsonImporter.Import(LoadJson());
        }

        [MenuItem("GameObject/UI/Json/Import")]
        private static void ImportAndBuild()
        {
            UIJsonImporter.ImportAndBuild(LoadJson());
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