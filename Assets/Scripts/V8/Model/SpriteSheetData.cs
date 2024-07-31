using System;

namespace V8
{
    [Serializable]
    public class SpriteSheetData
    {
        public string id;
        public string spriteId;
        public int[] cellSize;
        public int cellCount;
    }
}