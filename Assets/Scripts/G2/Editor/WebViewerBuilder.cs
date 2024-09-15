using System.IO;
using UnityEditor;
using UnityEngine;

namespace V8.Template.Editor
{
    public class WebViewerBuilder : MonoBehaviour
    {
        [MenuItem("Build/Build V8 Viewer")]
        public static void Build()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            }
            
            var path = Path.Combine(Application.dataPath, "../docs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            PlayerSettings.WebGL.template = "PROJECT:Viewer";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Viewer.unity" },
                locationPathName = path, 
                target = BuildTarget.WebGL,
                options = BuildOptions.AutoRunPlayer
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("[UIViewerBuilder.Build] Done !");
        }
    }
}