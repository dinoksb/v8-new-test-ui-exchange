using Newtonsoft.Json;

namespace G2.Model
{
    public class SpriteSheetData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;

        [JsonProperty("textureId", Required = Required.Always)]
        public string TextureId;

        [JsonProperty("pixelsPerUnit", Required = Required.Always)]
        public float PixelsPerUnit;

        [JsonProperty("offset", Required = Required.Default)]
        public int[] Offset;

        [JsonProperty("cellSize", Required = Required.Always)]
        public int[] CellSize;

        [JsonProperty("border", Required = Required.Default)]
        public int[] Border; // (0=left, 1=bottom, 2=right, 3=top)

        [JsonProperty("pivot", Required = Required.Default)]
        public float[] Pivot;

        [JsonProperty("multiple", Required = Required.Default)]
        public bool Multiple;
    }
}
