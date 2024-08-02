using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

namespace V8
{
    public class UIManager : MonoBehaviour
    {
        private IFactoryProvider<IElementFactory<IElement>> _factoryProvider;
        // private readonly Dictionary<string, IElement> _ui = new();
        private readonly Dictionary<string, Dictionary<string, ElementData>> _ui = new();
        private Dictionary<string, Sprite> _sprites = new();
        
        private IElement _canvas;
        private IElement Canvas => _canvas ??= new Canvas(GetType().Name);

        private void OnEvent(ulong clientId, string uiId, string eventTriggerType, string eventId)
        {
        }

        private void Clear()
        {
            // DestroyImmediate(Canvas.Self.gameObject);
            _canvas = null;
            Debug.Log("[Clear]");
        }

        public async void Load(string json)
        {
            Clear();
            var data = JsonConvert.DeserializeObject<UIData>(json, ElementDataConverter.SerializerSettings);
            _sprites = await SpriteImporter.Import(data.asset.sprite, true);
            
            // BuildUI(data.ui.Values);
            Debug.Log($"[Load] : {json}");
        }

        private void BuildUI(Dictionary<string, ElementData> uis)
        {
            // TODO: UI 생성 테스트 필요.
            // TODO: Element 초기화 시 선택적 데이터를 위해 반드시 Initialize 필요.
            // // foreach (var uiData in uis)
            // // {
            // //     var id = uiData.Key;
            // //     var element = uiData.Value;
            // //     CreateElement(element, Canvas, id);
            // // }
            // foreach (var (_, uiData) in uis)
            // {
            //     CreateElement(uiData, Canvas, id);
            // }
        }

        private IElement CreateElement(ElementData data, IElement parent, string id)
        {
            _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
            var factory = _factoryProvider.GetFactory(data.type);
            var element = factory.Create(data, parent, id);
            // Todo: 여기서 Parent UI Parent 를 연결해야함.
            element.Visible = data.visible;
            return element;
        }
        
        // private IElement CreateElement(ElementData data, IElement parent)
        // {
        //     _factoryProvider = new ElementFactoryProvider(_sprites, OnEvent);
        //     var factory = _factoryProvider.GetFactory(data.type);
        //     var element = factory.Create(data, parent);
        //     var children = CreateChildrenElement(data.children, element);
        //     element.Children.AddRange(children);
        //     element.Visible = data.visible;
        //     return element;
        // }

        // private IEnumerable<IElement> CreateChildrenElement(IEnumerable<ElementData> childrenData, IElement parent)
        // {
        //     return childrenData.Select(child => CreateElement(child, parent)).ToList();
        // }
        
        // public override UniTask<bool> Load(string verseAddress, string url, bool forceResourceDownload = false,
        //     CancellationToken cancellationToken = default)
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void Apply(string uiId, ElementData uiData)
        // {
        //     throw new System.NotImplementedException();
        // }

        // public override ElementData Get(string fileName)
        // {
        //     return null;
        // }
    }
}