using UnityEngine;

namespace V8.Template
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] private string url;
        [SerializeField] private UIImporter _uiImporter;
        private void Start()
        {
#if UNITY_EDITOR
            LoadUIAsync(url);
#endif
            
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif
        }

        private async void LoadUIAsync(string uiUrl)
        {
            var json = await WebRequestUtility.GetData(uiUrl);
            LoadUI(json);
        }

        public void LoadUI(string jsonData)
        {
            _uiImporter.Load(jsonData);
        }
    }
}
