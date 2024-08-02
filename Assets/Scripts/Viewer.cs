using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace V8.Template
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] private string url;
        [FormerlySerializedAs("_uiImporter")] [SerializeField] private UIManager uiManager;
        private void Start()
        {
#if UNITY_EDITOR
            // LoadUIAsync(url);
            LoadUIAsync();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif
        }
        private async void LoadUIAsync()
        {
            Debug.Log(Application.streamingAssetsPath);
            var uiJsonPath = Path.Combine(Application.streamingAssetsPath, "ui-test.json");
            var json = await File.ReadAllTextAsync(uiJsonPath);
            LoadUI(json);
        }
        
        private async void LoadUIAsync(string uiUrl)
        {
            var json = await WebRequestUtility.GetData(uiUrl);
            LoadUI(json);
        }

        public void LoadUI(string jsonData)
        {
            uiManager.Load(jsonData);
        }
    }
}
