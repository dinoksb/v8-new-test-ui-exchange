using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using G2.Model.UI;
using Utilities;
using ConstraintType = G2.Model.UI.FrameData.ConstraintType;

namespace G2.UI.Elements.Basic
{
    public class Label : Frame
    {
        private TMP_Text _tmp;
        private TransformLinkComponent _transformLink;

        private const string DefaultFontAssetPath = "Fonts & Materials";
        private const string DefaultFontAsset = "LiberationSans SDF";

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
                string prevText = _tmp.text;
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
                // todo: path 가 현재는 'Fonts & Materials'로 고정인데 어떻게 해야할지 논의 필요.
                // 1. font 데이터를 어떻게 관리 할건지?
                //  - 프로젝트 안에 특정한 폴더안에 넣어서 관리 할 것인지
                //  - 런타임에 ttf 파일을 불러 들여와서 생성 해서 사용 할 것인지? (동작 검증 필요)
                //  - Font.asset 을 에셋번들화 하여 불러올 것인지?
                _tmp.font = GetFontAsset(value);
            }
        }

        public ConstraintType Constraint { get; set; }

        public bool AutoSize { get; set; }

        private Vector2 _screenResolution = new(Screen.width, Screen.height);
        private Vector2 _referenceResolution;

        private readonly List<TextChangeEventAction> _textChangeEvents = new();

        public delegate void TextChangeEventAction(IElement element, string prevText, string newText);

        public Label(string uid, LabelData data, LabelComponents components, Vector2 referenceResolution) : base(uid,
            data, components)
        {
            _tmp = components.TMP;
            _referenceResolution = referenceResolution;
            _transformLink = components.TransformLinkComponent;
            SetTransformLink(_transformLink);
            SetEvents();
            SetValues(data);
        }

        public override IElement Copy(RectTransform self, IElement parent)
        {
            var clone = (Label)base.Copy(self, parent);
            clone._tmp = self.GetComponent<TextMeshProUGUI>();
            if (_transformLink)
            {
                clone._transformLink = _transformLink;
                clone.SetTransformLink(clone._transformLink);
            }

            clone.Parent.OnMoveFront += clone.MoveFront;
            return clone;
        }

        public override void Update(ElementData data)
        {
            base.Update(data);
            var labelData = (LabelData)data;
            SetValues(labelData);
        }

        public override void MoveFront()
        {
            _transformLink.Self.SetAsLastSibling();
            base.MoveFront();
        }

        private void SetValues(LabelData data)
        {
            Interactable = data.interactable;
            FontId = data.fontId;
            TextAlignment = TypeConverter.ToTextAlignmentOptions(data.textAlignment);
            FontColor = TypeConverter.ToColor(data.fontColor);
            FontSize = SetFontSize(data.fontSize, false);
            //FontSize = SetFontSize(data.fontSize, AutoSize);
            Text = data.text;
            // Todo: 추후 추가 될 수 있는 데이터.
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
            if (CheckIsCanvas(Parent)) return;
            Parent.OnMoveFront += MoveFront;
            Parent.AddVisibleChangedListener(VisibleChanged);
        }

        private float SetFontSize(float size, bool isAutoSize)
        {
            return isAutoSize
                ? size / CalculateCanvasScaleFactor(_referenceResolution, _screenResolution, Constraint)
                : size;
        }

        // private TMP_FontAsset GetFontAssetFromResources(string id)
        // {
        //     var fontAsset = Resources.Load(id, typeof(TMP_FontAsset)) as TMP_FontAsset;
        //     if (fontAsset == null)
        //     {
        //         var defaultFontPath = $"{DefaultFontAssetPath}/{DefaultFontAsset}";
        //         fontAsset = GetFontAssetFromResources($"{DefaultFontAssetPath}/{DefaultFontAsset}");
        //         InternalDebug.LogWarning($"The [{id}] font does not exist. Replace with default [{defaultFontPath}]font.");
        //     }
        //
        //     return fontAsset;
        // }

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

        private TMP_FontAsset GetFontAsset(string id)
        {
            string fontPath;
            var filePaths = FindFilesInProject(Application.dataPath, id);
            if (filePaths.Count == 0)
            {
                fontPath = $"{DefaultFontAssetPath}/{DefaultFontAsset}";
                InternalDebug.Log($"The font [{id}] does not exist. Replace with default font [{DefaultFontAsset}].");
            }
            else
            {
                var filePath = filePaths[0];
                var startToken = @"Resources\";
                var endToken = ".asset";

                int startIndex = filePath.IndexOf(startToken, StringComparison.Ordinal) + startToken.Length;
                int endIndex = filePath.IndexOf(endToken, startIndex, StringComparison.Ordinal);
                fontPath = filePath.Substring(startIndex, endIndex - startIndex);
            }

            var fontAsset = Resources.Load(fontPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
            return fontAsset;
        }

        private List<string> FindFilesInProject(string rootPath, string targetFileName)
        {
            List<string> result = new List<string>();

            try
            {
                foreach (string dir in Directory.GetDirectories(rootPath))
                {
                    string[] files = Directory.GetFiles(dir, $"{targetFileName}.asset", SearchOption.AllDirectories);
                    result.AddRange(files);
                }
            }
            catch (Exception ex)
            {
                InternalDebug.LogException(ex);
            }

            return result;
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