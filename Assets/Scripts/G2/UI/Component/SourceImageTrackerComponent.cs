using UnityEngine;
using UnityEngine.UI;

namespace G2.UI.Component
{
    public class SourceImageTrackerComponent : MonoBehaviour
    {
        public Image SourceImage { get; private set; }

        public void Initailize(Image image)
        {
            Debug.Assert(image != null, "image must not be null");
            SourceImage = image;
        }
    }
}
