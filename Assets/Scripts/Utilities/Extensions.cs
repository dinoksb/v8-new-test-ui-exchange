using UnityEngine;

namespace Utilities
{
    public static class Extensions
    {
        public static Vector2 ToReverseYAxis(this Vector2 vec)
        {
            return new Vector2(vec.x, 1 - vec.y);
        }
    }
}


