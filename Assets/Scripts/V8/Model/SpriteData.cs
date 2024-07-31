using System;

namespace V8
{
    [Serializable]
    public class SpriteData
    {
        public string url;
        public string id;
        public float pixelsPerUnit;
        public int[] offset;
        public int[] size;
        public int[] border; // (0=left, 1=bottom, 2=right, 3=top)
        public float[] pivot;
    }
}