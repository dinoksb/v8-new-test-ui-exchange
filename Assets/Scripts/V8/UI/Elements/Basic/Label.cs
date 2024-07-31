using TMPro;
using UnityEngine;

namespace V8
{
    public class Label : Layout
    {
        private TMP_Text _tmp;
        private string _fontId;

        public TextAlignmentOptions TextAlignment
        {
            get => _tmp.alignment;
            set => _tmp.alignment = value;
        }

        public Color FontColor
        {
            get => _tmp.color;
            set => _tmp.color = value;
        }

        public float FontSize
        {
            get => _tmp.fontSize;
            set
            {
                _tmp.fontSize = value;
                _tmp.fontSizeMax = value;
            }
        }

        public float MinFontSize
        {
            get => _tmp.fontSizeMin;
            set => _tmp.fontSizeMin = value;
        }

        public float CharacterSpacing
        {
            get => _tmp.characterSpacing;
            set => _tmp.characterSpacing = value;
        }

        public float LineSpacing
        {
            get => _tmp.lineSpacing;
            set => _tmp.lineSpacing = value;
        }

        public bool AutoSize
        {
            get => _tmp.enableAutoSizing;
            set => _tmp.enableAutoSizing = value;
        }

        public bool SingleLine
        {
            get => _tmp.enableWordWrapping;
            set => _tmp.enableWordWrapping = value;
        }

        public bool Ellipsis
        {
            get => _tmp.overflowMode == TextOverflowModes.Ellipsis;
            set => _tmp.overflowMode = value ? TextOverflowModes.Ellipsis : TextOverflowModes.Overflow;
        }

        public bool Bold
        {
            get => (_tmp.fontStyle & FontStyles.Bold) == FontStyles.Bold;
            set
            {
                if (value)
                    _tmp.fontStyle |= FontStyles.Bold;
                else
                    _tmp.fontStyle &= ~FontStyles.Bold;
            }
        }

        public bool Italic
        {
            get => (_tmp.fontStyle & FontStyles.Italic) == FontStyles.Italic;
            set
            {
                if (value)
                    _tmp.fontStyle |= FontStyles.Italic;
                else
                    _tmp.fontStyle &= ~FontStyles.Italic;
            }
        }

        public bool Underline
        {
            get => (_tmp.fontStyle & FontStyles.Underline) == FontStyles.Underline;
            set
            {
                if (value)
                    _tmp.fontStyle |= FontStyles.Underline;
                else
                    _tmp.fontStyle &= ~FontStyles.Underline;
            }
        }

        public bool Strikethrough
        {
            get => (_tmp.fontStyle & FontStyles.Strikethrough) == FontStyles.Strikethrough;
            set
            {
                if (value)
                    _tmp.fontStyle |= FontStyles.Strikethrough;
                else
                    _tmp.fontStyle &= ~FontStyles.Strikethrough;
            }
        }

        public string Text
        {
            get => _tmp.text;
            set => _tmp.text = value;
        }

        public Label(LabelData data, LabelComponents components) : base(data, components)
        {
            _tmp = components.TMP;
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Label)base.Copy(self, parent);
            var childSelf = GetChildSelf(self, UIConfig.Element);
            clone._tmp = childSelf.GetComponent<TextMeshProUGUI>();
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var labelData = (LabelData)data;
            SetValues(labelData);
        }
        
        private void SetValues(LabelData data)
        {
            TextAlignment = TypeConverter.ToTextAlignmentOptions(data.textAlignment);
            FontColor = TypeConverter.ToColor(data.fontColor);
            FontSize = data.fontSize;
            MinFontSize = data.minFontSize;
            CharacterSpacing = data.characterSpacing;
            LineSpacing = data.lineSpacing;
            AutoSize = data.autoSize;
            SingleLine = data.singleLine;
            Ellipsis = data.ellipsis;
            Bold = data.bold;
            Italic = data.italic;
            Underline = data.underline;
            Strikethrough = data.strikethrough;
            Text = data.text;
        }
    }
}