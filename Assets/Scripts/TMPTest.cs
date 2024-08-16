using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using V8.Utilities;

public class TMPTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmp_text;
    [SerializeField] private string _fontId;

    
    [ContextMenu("TEST")]
    public void Test()
    {
        var font = Resources.Load($"Fonts & Materials/{_fontId}", typeof(TMP_FontAsset)) as TMP_FontAsset;
        InternalDebug.Log("font: " + font);
        InternalDebug.Log("font.name: " + font.name);
        InternalDebug.Log("font.material: " + font.material);

        _tmp_text.font = font;
        
        InternalDebug.Log("_tmp_text.font: " + _tmp_text.font);
        InternalDebug.Log("_tmp_text.font.name: " + _tmp_text.font.name);
    }

    [ContextMenu("Find File Test")]
    public void FindFileTest()
    {
        var fontAsset = GetFontAsset("LiberationSans SDF");
        
        InternalDebug.Log("fontAsset: " + fontAsset);
    }

    private TMP_FontAsset GetFontAsset(string id)
    {
        var filePaths = FindFilesInProject(Application.dataPath, id);
        
        // TMP_FontAsset fontAsset = string.
        if (filePaths.Count == 0)
        {
            Debug.Log($"The font does not exist. Replace with default font.");
        }

        var filePath = filePaths[0];
        var startToken = @"Resources\";
        var endToken = ".asset";

        int startIndex = filePath.IndexOf(startToken, StringComparison.Ordinal) + startToken.Length;
        int endIndex = filePath.IndexOf(endToken, startIndex, StringComparison.Ordinal);

        string fontPath = filePath.Substring(startIndex, endIndex - startIndex);
        
        return Resources.Load(fontPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
    }

    private List<string> FindFilesInProject(string rootPath, string targetFileName)
    {
        List<string> result = new List<string>();

        try
        {
            foreach (string dir in Directory.GetDirectories(rootPath))
            {
                string[] files = Directory.GetFiles(dir, $"{targetFileName}.asset", SearchOption.AllDirectories);
                result.AddRange(files);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return result;
    }
}
