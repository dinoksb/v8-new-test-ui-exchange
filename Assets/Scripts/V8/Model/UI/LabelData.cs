using System;

namespace V8
{
    [Serializable]
    public class LabelData : LayoutData
    {
        public string textAlignment;
        public string fontId;
        public float[] fontColor;
        public float fontSize;
        public float minFontSize;
        public float characterSpacing;
        public float lineSpacing;
        public bool autoSize;
        public bool singleLine;
        public bool ellipsis;
        public bool bold;
        public bool italic;
        public bool underline;
        public bool strikethrough;
        public string text;
    }
}