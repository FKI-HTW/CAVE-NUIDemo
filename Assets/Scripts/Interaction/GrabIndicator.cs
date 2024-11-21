using UnityEngine;
using UnityEngine.UI;

namespace HTW.CAVE.Interaction
{
    public class GrabIndicator : MonoBehaviour
    {
        public Image indicatorImage;
        private float _timer = 0f;
        private bool _isGrabbing = false;

        private void Update()
        {
            if (_isGrabbing)
            {
                _timer += Time.deltaTime;

                float fillAmount = _timer / 1.5f; // Calculate the fill amount based on the timer and desired duration
                indicatorImage.fillAmount = fillAmount;

                if (_timer >= 1.5f)
                {
                    // Logic for when the grab is completed
                    StopGrabIndicator();
                }
            }
        }

        public void StartGrabIndicator()
        {
            _isGrabbing = true;
            _timer = 0f;
            indicatorImage.fillAmount = 0f; // Reset the indicator fill amount when starting a new grab
        }

        public void StopGrabIndicator()
        {
            _isGrabbing = false;
            _timer = 0f;
            indicatorImage.fillAmount = 0f; // Reset the indicator fill amount when stopping the grab
        }
    }
}