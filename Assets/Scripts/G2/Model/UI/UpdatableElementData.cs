using System;
using Newtonsoft.Json;

namespace G2.Model.UI
{
    [Serializable]
    public abstract class UpdatableElementData : ElementData
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
    }
}
