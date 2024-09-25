using UnityEngine;

namespace Utilities
{
    public static class Extensions
    {
        public static Vector2 ToReverseYAxis(this Vector2 vec)
        {
            return new Vector2(vec.x, 1 - vec.y);
        }
        
        public static bool ApproximatelyEqual(this Vector2 v1, Vector2 v2)
        {
            return Vector2.Distance(v1, v2) < Mathf.Epsilon;
        }

        public static bool ApproximatelyEqual(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Distance(v1, v2) < Mathf.Epsilon;
        }
        
        public static bool ApproximatelyEqual(this Quaternion q1, Quaternion q2)
        {
            return Quaternion.Angle(q1, q2) < Mathf.Epsilon;
        }

        public static int[] ToIntArray(this Vector4 vec)
        {
            return new int[] { Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z), Mathf.RoundToInt(vec.w) };
        }

        public static Color To255(this Color color)
        {
            float r = color.r <= 1.0 ? color.r * 255f : color.r;
            float g = color.g <= 1.0 ? color.g * 255f : color.g;
            float b = color.b <= 1.0 ? color.b * 255f : color.b;
            float a = color.a <= 1.0 ? color.a * 255f : color.a;
            return new Color(r, g, b, a);
        }
        
        public static Color To01(this Color color)
        {
            float r = color.r > 1.0 ? color.r / 255f : color.r;
            float g = color.g > 1.0 ? color.g / 255f : color.g;
            float b = color.b > 1.0 ? color.b / 255f : color.b;
            float a = color.a > 1.0 ? color.a / 255f : color.a;
            return new Color(r, g, b, a);
        }
    }
}
