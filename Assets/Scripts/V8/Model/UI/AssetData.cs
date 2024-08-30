using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    // todo: SpriteImporter 에서 TextureData 의 url 로 다운로드 받을 수 있도록 로직 수정필요.
    // todo: sprite 를 사용하는 부분 로직 수정 필요한지 확인.
    [Serializable]
    public struct AssetData
    {
        public Dictionary<string, TextureData> texture;
        // Note: sprite 값도 default 값인 경우가 있을지? (null 인 경우 있을수도 있을것 같음...)
        /// <summary>
        /// key: ID
        /// value: SpriteData
        /// </summary>
        public Dictionary<string, SpriteData> sprite;
        // public List<SpriteData> sprite;
    }
}