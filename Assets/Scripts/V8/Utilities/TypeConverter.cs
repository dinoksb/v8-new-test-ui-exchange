using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace V8
{
    public static class TypeConverter
    {
        public static int ToInteger(string value)
        {
            return int.TryParse(value, out var parsedValue) ? parsedValue : 0;
        }
        
        public static float ToFloat(string value)
        {
            return float.TryParse(value, out var parsedValue) ? parsedValue : 0;
        }
   
        public static float ToRatio(string value)
        {
            return float.TryParse(value, out var percent) ? Math.Clamp(percent, 0, 100) * 0.01f : 0;
        }
        
        public static Color ToColor(float[] values)
        {
            if (values == null || values.Length < 4)
            {
                return Color.clear;
            }
            
            float r = values[0] <= 1.0 ? values[0] * 255f : values[0];
            float g = values[1] <= 1.0 ? values[1] * 255f : values[1];
            float b = values[2] <= 1.0 ? values[2] * 255f : values[2];
            float a = values[3] <= 1.0 ? values[3] * 255f : values[3];

            return new Color(values[0], values[1], values[2], values[3]);
        }
        
        public static Color32 ToColor32(float[] values)
        {
            if (values == null || values.Length < 4)
            {
                return new Color32(0,0,0,0);
            }

            return new Color32((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);
        }
        
        public static Color ToColor0To1(float[] values)
        {
            if (values == null || values.Length < 4)
            {
                return Color.clear;
            }

            float r = values[0] > 1.0 ? values[0] / 255f : values[0];
            float g = values[1] > 1.0 ? values[1] / 255f : values[1];
            float b = values[2] > 1.0 ? values[2] / 255f : values[2];
            float a = values[3] > 1.0 ? values[3] / 255f : values[3];

            return new Color(r, g, b, a);
        }
        
        public static Vector2 ToVector2(int[] values)
        {
            if (values == null || values.Length < 2)
            {
                return Vector2.zero;
            }

            return new Vector2(values[0], values[1]);
        }
        
        public static Vector2 ToVector2(IReadOnlyList<float> values)
        {
            if (values == null || values.Count < 2)
            {
                return Vector2.zero;
            }

            return new Vector2(values[0], values[1]);
        }
   
        public static Vector4 ToVector4(IReadOnlyList<int> values)
        {
            if (values == null || values.Count < 4)
            {
                return Vector4.zero;
            }

            return new Vector4(values[0], values[1], values[2], values[3]);
        }
        
        public static Vector4 ToVector4(IReadOnlyList<float> values)
        {
            if (values == null || values.Count < 4)
            {
                return Vector4.zero;
            }

            return new Vector4(values[0], values[1], values[2], values[3]);
        }
        
        public static RectOffset ToRectOffset(IReadOnlyList<int> values)
        {
            if (values == null || values.Count < 4)
            {
                return new RectOffset();
            }

            return new RectOffset(values[0], values[1], values[2], values[3]);
        }

        public static TextAlignmentOptions ToTextAlignmentOptions(string value)
        {
            return Enum.TryParse(value, out TextAlignmentOptions result) ? result : TextAlignmentOptions.TopLeft;
        }
        
        public static TextAnchor ToTextAnchor(string value)
        {
            return Enum.TryParse(value, out TextAnchor result) ? result : TextAnchor.UpperLeft;
        }

        public static GridLayoutGroup.Constraint ToConstraint(string value)
        {
            return Enum.TryParse(value, out GridLayoutGroup.Constraint result) ? result : GridLayoutGroup.Constraint.Flexible;
        }
        
        public static GridLayoutGroup.Corner ToCorner(string value)
        {
            return Enum.TryParse(value, out GridLayoutGroup.Corner result) ? result : GridLayoutGroup.Corner.UpperLeft;
        }

        public static GridLayoutGroup.Axis ToAxis(string value)
        {
            return Enum.TryParse(value, out GridLayoutGroup.Axis result) ? result : GridLayoutGroup.Axis.Horizontal;
        }

        public static bool TryEventTriggerType(string value, out EventTriggerType eventTriggerType)
        {
            return Enum.TryParse(value, out eventTriggerType);
        }
        
        public static Sprite ToSprite(
            Texture2D texture,
            Vector2 offset,
            Vector2 size,
            Vector4 border, 
            Vector2 pivot,
            string name,
            float pixelsPerUnit)
        {
            var width = Mathf.Min(size.x, texture.width - offset.x);
            var height = Mathf.Min(size.y, texture.height - offset.y);
            var rect = new Rect(offset.x, offset.y, width, height);
            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            sprite.name = name;
            return sprite;
        }
    }
}