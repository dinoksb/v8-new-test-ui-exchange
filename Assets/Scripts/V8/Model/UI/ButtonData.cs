using System;
using System.Collections.Generic;

namespace V8
{
    [Serializable]
    public class ButtonData : LayoutData
    {
        public Dictionary<string, string> events;
        public float threshold;
    }
}