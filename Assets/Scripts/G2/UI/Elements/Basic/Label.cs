using System;
using System.Collections.Generic;
using G2.Model.UI;
using G2.UI.Component;
using TMPro;
using UnityEngine;
using Utilities;

namespace G2.UI.Elements.Basic
{
    public class Label : UpdatableElement
    {
        public delegate void TextChangeEventAction(IElement element, string prevText, string newText);
        
        private const string _DEFAULT_FONT_ASSET_PATH = "Fonts & Materials";
        private const string _DEFAULT_FONT_ASSET = "LiberationSans SDF";

        private readonly List<TextChangeEventAction> _textChangeEvents = new();

        private readonly Vector2 _screenResolution = new(Screen.width, Screen.height);
        private readonly Vector2 _referenceResolution;
        
        private TMP_Text _tmp;
        private TransformLinkComponent _elementTransformLink;

        public bool AutoSize { get; set; }
        
        public override bool Interactable
        {
            get => _tmp.raycastTarget;
            set => _tmp.raycastTarget = value;
        }

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
            set
            {
                var prevText = _tmp.text;
                _tmp.text = value;
                foreach (var eventAction in _textChangeEvents)
                {
                    eventAction.Invoke(this, prevText, value);
                }
            }
        }

        private string FontId
        {
            get => _tmp.font.name;
            set
            {
                // todo: The path is currently fixed to 'Fonts & Materials'; we need to discuss how to handle this.
                _tmp.font = GetFontAsset(value);
            }
        }

        public Label(string uid, LabelData data, LabelComponents components, Vector2 referenceResolution) : base(uid,
            data, components)
        {
            _tmp = components.TMP;
            _referenceResolution = referenceResolution;
            _elementTransformLink = components.ElementTransformLinkComponent;
            SetTransformLink(_elementTransformLink);
            SetEvents();
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, RectTransform parentRectTransform, IElement parentElement)
        {
            var clone = (Label)base.Copy(self, parentRectTransform, parentElement);
            clone._tmp = self.GetComponent<TextMeshProUGUI>();
            if (_elementTransformLink)
            {
                clone._elementTransformLink = _elementTransformLink;
                clone.SetTransformLink(clone._elementTransformLink);
            }

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
            Interactable = data.interactable;
            FontId = data.fontId;
            TextAlignment = TypeConverter.ToTextAlignmentOptions(data.textAlignment);
            FontColor = TypeConverter.ToColor(data.fontColor);
            FontSize = SetFontSize(data.fontSize, false);
            Text = data.text;
            // Todo: Data that may be added in the future.
            //FontSize = SetFontSize(data.fontSize, AutoSize);
            // CharacterSpacing = data.characterSpacing;
            // LineSpacing = data.lineSpacing;
            // AutoSize = data.autoSize;
            // Constraint = data.fontSizeConstraint;
            // SingleLine = data.singleLine;
            // Ellipsis = data.ellipsis;
            // Bold = data.bold;
            // Italic = data.italic;
            // Underline = data.underline;
            // Strikethrough = data.strikethrough;
        }

        private void SetEvents()
        {
            if (Parent == null) return;
            Parent.OnPositionUpdated += PositionChanged;
            Parent.AddVisibleChangedListener(VisibleChanged);
        }

        private float SetFontSize(float size, bool isAutoSize)
        {
            return isAutoSize
                ? size / CalculateCanvasScaleFactor(_referenceResolution, _screenResolution, constraintType)
                : size;
        }

        // private TMP_FontAsset GetFontAssetFromResources(string id)
        // {
        //     var fontAsset = Resources.Load(id, typeof(TMP_FontAsset)) as TMP_FontAsset;
        //     if (fontAsset == null)
        //     {
        //         var defaultFontPath = $"{DefaultFontAssetPath}/{DefaultFontAsset}";
        //         fontAsset = GetFontAssetFromResources($"{DefaultFontAssetPath}/{DefaultFontAsset}");
        //         Debug.LogWarning($"The [{id}] font does not exist. Replace with default [{defaultFontPath}]font.");
        //     }
        //
        //     return fontAsset;
        // }

        private float CalculateCanvasScaleFactor(Vector2 referenceResolution, Vector2 targetResolution, UpdatableElementData.ConstraintType constraint)
        {
            // Calculate horizontal and vertical scale
            var widthScale = targetResolution.x / referenceResolution.x;
            var heightScale = targetResolution.y / referenceResolution.y;
            float calcScaleFactor;

            // Calculate scale factor based on constraint value
            switch (constraint)
            {
                case UpdatableElementData.ConstraintType.XX:
                    // Calculate scale factor considering only horizontal resolution
                    calcScaleFactor = widthScale;
                    break;
                case UpdatableElementData.ConstraintType.YY:
                    // Calculate scale factor considering only vertical resolution
                    calcScaleFactor = heightScale;
                    break;
                case UpdatableElementData.ConstraintType.XY:
                default:
                    throw new ArgumentOutOfRangeException(nameof(constraintType), $"Unhandled ConstraintType enum value: {constraintType}");
            }

            return calcScaleFactor;
        }

        private static TMP_FontAsset GetFontAsset(string id)
        {
            string fontPath;
            var filePaths = Util.FindFilePaths(Application.dataPath, id);
            if (filePaths.Count == 0)
            {
                fontPath = $"{_DEFAULT_FONT_ASSET_PATH}/{_DEFAULT_FONT_ASSET}";
                Debug.Log($"The font [{id}] does not exist. Replace with default font [{_DEFAULT_FONT_ASSET}].");
            }
            else
            {
                var filePath = filePaths[0];
                const string START_TOKEN = @"Resources\";
                const string END_TOKEN = ".asset";

                var startIndex = filePath.IndexOf(START_TOKEN, StringComparison.Ordinal) + START_TOKEN.Length;
                var endIndex = filePath.IndexOf(END_TOKEN, startIndex, StringComparison.Ordinal);
                fontPath = filePath.Substring(startIndex, endIndex - startIndex);
            }

            var fontAsset = Resources.Load(fontPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
            return fontAsset;
        }

        public void AddTextChangedListener(TextChangeEventAction eventAction)
        {
            _textChangeEvents.Add(eventAction);
        }

        public void RemoveTextChangedListener(TextChangeEventAction eventAction)
        {
            _textChangeEvents.Remove(eventAction);
        }

        public void RemoveAllTextChangedListener()
        {
            _textChangeEvents.Clear();
        }
    }
}
