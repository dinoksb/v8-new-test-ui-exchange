using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace V8
{
    public abstract class BaseUIManager : IUIManager
    {
        // public abstract UniTask<bool> Load(string verseAddress, string url, bool forceResourceDownload = false, CancellationToken cancellationToken = default);
        //
        // public abstract void Apply(string uiId, ElementData uiData);

        public abstract ElementData Get(string fileName);

        protected async UniTask<UIData> GetData(string url)
        {
            var json = await WebRequestUtility.GetData(url);
            var data = JsonConvert.DeserializeObject<UIData>(json, ElementDataConverter.SerializerSettings);
            return data;
        }
    }
}