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
    }
}


