using UnityEngine;

namespace HTW.CAVE.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        public bool isGrabbed = false;
        public GrabInteractor grabInteractor;
        public GrabIndicator grabIndicator;

        public void Start()
        {
            grabIndicator = GetComponentInChildren<GrabIndicator>();
        }

        public void OnTouchEnter()
        {
            // Logic for when the object is touched or entered by the hand
        }

        public void OnTouchExit()
        {
            // Logic for when the object is no longer touched or exited by the hand
        }

        public void OnGrab()
        {
            if (!isGrabbed)
            {
                Debug.Log("Grab");
                isGrabbed = true;
                grabInteractor.GrabObject();
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = true;
                }
            }
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            if (isGrabbed)
            {
                transform.position = newPosition;
            }
        }

        public void Release()
        {
            isGrabbed = false;
            grabInteractor = null;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }
    }
}