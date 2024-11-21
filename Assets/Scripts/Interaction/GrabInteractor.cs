namespace HTW.CAVE.Interaction
{
    using UnityEngine;

    public class GrabInteractor : MonoBehaviour
    {
        /// <summary>
        /// currently touched InteractableObject
        /// </summary>
        private InteractableObject _currentInteractable;
        private float _touchDuration = 0f;
        private bool _isTouching = false;
        private bool _isGrabbing = false;
        private Vector3 _initialGrabOffset;
        public KeywordRecognition keywordRecognition;


        private void Start()
        {
            keywordRecognition = GameObject.Find("Interaction").GetComponent<KeywordRecognition>();
            if(keywordRecognition != null )
            {
                keywordRecognition.OnKeywordRecognized += UnGrabInteractableObject;
            }
        }

        private void UnGrabInteractableObject(UnityEngine.Windows.Speech.PhraseRecognizedEventArgs obj)
        {
            if (obj.text.Equals("drop") && _isGrabbing)
            {
                _currentInteractable.Release();
                _currentInteractable = null;
                _isGrabbing = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTouching || _isGrabbing)
                return;

            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null && !interactable.isGrabbed && interactable.grabInteractor == null)
            {
                _currentInteractable = interactable;
                _currentInteractable.OnTouchEnter();
                _currentInteractable.grabInteractor = this;
                _isTouching = true;
                _touchDuration = 0f;
                _currentInteractable.grabIndicator.StartGrabIndicator();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isGrabbing)
                return;
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (_currentInteractable == interactable)
                {
                    _currentInteractable.OnTouchExit();
                    _currentInteractable.grabIndicator.StopGrabIndicator();
                    _currentInteractable.grabInteractor = null;
                    _currentInteractable = null;
                    _isTouching = false;
                    _touchDuration = 0f;
                }
            }
        }

        private void Update()
        {
            if (_isTouching && !_isGrabbing)
            {
                _touchDuration += Time.deltaTime;

                if (_touchDuration >= 1.5f)
                {
                    //currentInteractable.grabInteractor = this;
                    _currentInteractable.OnGrab();
                    _isTouching = false;
                    _touchDuration = 0f;
                    _currentInteractable.grabIndicator.StopGrabIndicator();
                }

            }
            if (_isGrabbing)
            {
                UpdateGrabbedObjectPosition();
            }
        }

        public void GrabObject()
        {
            //Debug.Log("Interactor is grabbing " + interactable.gameObject.name);
            _isGrabbing = true;
            //currentInteractable = interactable;
            //currentInteractable.OnGrab();
            _initialGrabOffset = _currentInteractable.transform.position - transform.position;
        }

        private void UpdateGrabbedObjectPosition()
        {
            if (_currentInteractable != null)
            {
                Debug.Log("Updating position");
                Vector3 newPosition = transform.position + _initialGrabOffset;
                _currentInteractable.UpdatePosition(newPosition);
            }
        }

        private void ReleaseGrabbedObject()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Release();
                _currentInteractable = null;
                _isGrabbing = false;
            }
        }
    }
}