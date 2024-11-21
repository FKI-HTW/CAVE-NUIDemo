using HTW.CAVE.Interaction;
using UnityEngine;

namespace HTW.CAVE.Locomotion
{
    public class RotatePlayer : MonoBehaviour
    {
        public KeywordRecognition recognition;
        public Transform virtualEnvironment;
        public float rotationAmount = 90f;
        // Start is called before the first frame update
        void Start()
        {
            recognition.OnKeywordRecognized += Rotate;
        }

        private void Rotate(UnityEngine.Windows.Speech.PhraseRecognizedEventArgs obj)
        {
            if (obj.text.Equals("links") || obj.text.Equals("left"))
            {
                virtualEnvironment.Rotate(Vector3.up, -rotationAmount);
            }
            else if (obj.text.Equals("rechts") || obj.text.Equals("right"))
            {
                virtualEnvironment.Rotate(Vector3.up, rotationAmount);
            }
        }
    }
}