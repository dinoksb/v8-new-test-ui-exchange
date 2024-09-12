using System;
using Newtonsoft.Json;

namespace V8
{
    [Serializable]
    public class FrameData : ElementData
    {
        public enum ConstraintType
        {
            XX,
            XY,
            YY
        }
        
        [JsonProperty("interactable", Required = Required.Default)]
        public bool interactable = true;

        [JsonProperty("sizeConstraint", Required = Required.Default)]
        public ConstraintType sizeConstraint = ConstraintType.XY;
        
        [JsonProperty("dim", Required = Required.Default)]
        public float dim = 0;
    }
}