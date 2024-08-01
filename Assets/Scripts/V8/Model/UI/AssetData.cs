using System;
using System.Collections.Generic;
using UnityEngine;

namespace V8
{
    [Serializable]
    public struct AssetData
    {
        // Note: sprite 값도 default 값인 경우가 있을지? (null 인 경우 있을수도 있을것 같음...)
        
        /// <summary>
        /// key: ID
        /// value: SpriteData
        /// </summary>
        public Dictionary<string, SpriteData> sprite;
        // public List<SpriteData> sprite;
    }
}