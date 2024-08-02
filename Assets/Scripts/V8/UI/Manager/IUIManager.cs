using System.Threading;
using Cysharp.Threading.Tasks;

namespace V8
{
    public interface IUIManager
    {
        // public UniTask<bool> Load(string verseAddress, string url, bool forceResourceDownload = false, CancellationToken cancellationToken = default);
        // public void Apply(string uiId, ElementData uiData);
        public ElementData Get(string fileName);
    }
}