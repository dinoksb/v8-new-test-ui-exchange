using System;
using System.Collections.Generic;

namespace V8
{
    [Serializable]
    public struct UIData
    {
        public AssetData asset;
        public Dictionary<string, ElementData> ui;
    }
}