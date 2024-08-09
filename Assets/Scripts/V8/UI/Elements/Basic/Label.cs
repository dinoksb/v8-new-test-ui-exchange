using System;
using UnityEngine;
using TMPro;
using ConstraintType = V8.FrameData.ConstraintType;

namespace V8
{
    public class Label : Frame
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

        public ConstraintType Constraint { get; set; }

        public bool AutoSize { get; set; }

        private Vector2 _screenResolution = new(Screen.width, Screen.height);
        private Vector2 _referenceResolution;

        public Label(LabelData data, LabelComponents components, Vector2 referenceResolution) : base(data, components)
        {
            _tmp = components.TMP;
            _referenceResolution = referenceResolution;
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
            CharacterSpacing = data.characterSpacing;
            LineSpacing = data.lineSpacing;
            AutoSize = data.autoSize;
            Constraint = data.fontSizeConstraint;
            FontSize = SetFontSize(data.fontSize, AutoSize);
            SingleLine = data.singleLine;
            Ellipsis = data.ellipsis;
            Bold = data.bold;
            Italic = data.italic;
            Underline = data.underline;
            Strikethrough = data.strikethrough;
            Text = data.text;
            _tmp.raycastTarget = Interactable;
        }

        private float SetFontSize(float size, bool isAutoSize)
        {
            return isAutoSize
                ? size
                : size / CalculateCanvasScaleFactor(_referenceResolution, _screenResolution, Constraint);
        }

        private float CalculateCanvasScaleFactor(Vector2 referenceResolution, Vector2 targetResolution,
            ConstraintType constraint)
        {
            // 가로 및 세로 스케일 계산
            float widthScale = targetResolution.x / referenceResolution.x;
            float heightScale = targetResolution.y / referenceResolution.y;
            float calcScaleFactor = 1.0f;

            // Constraint 값에 따른 스케일 팩터 계산
            switch (constraint)
            {
                case ConstraintType.XX:
                    // 가로 해상도만 고려하여 스케일 팩터 계산
                    calcScaleFactor = widthScale;
                    break;
                case ConstraintType.YY:
                    // 세로 해상도만 고려하여 스케일 팩터 계산
                    calcScaleFactor = heightScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ConstraintType),
                        $"처리할 수 없는 ConstraintType enum 값입니다: {ConstraintType}");
            }

            return calcScaleFactor;
        }
    }
}