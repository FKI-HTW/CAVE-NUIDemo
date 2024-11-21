using HTW.CAVE.Kinect;
using HTW.CAVE.Interaction;
using System.Collections;
using UnityEngine;

namespace HTW.CAVE.Locomotion
{
    public class Teleport : MonoBehaviour
    {
        public LayerMask teleportLayers;
        public KeywordRecognition recognition;
        
        public KinectTracker kinectTracker;
        
        private KinectSkeleton _kinectSkeleton;
        private KinectSkeletonJoint _shoulderJointRight;
        private KinectSkeletonJoint _elbowJointRight;
        private KinectSkeletonJoint _wristJointRight;
        private KinectSkeletonJoint _wristJointLeft;
        private KinectSkeletonJoint _headJoint;

        private LineRenderer _lineRenderer;

        private Collider _armStraightCollider;
        private Collider _leftHandCollider;

        private bool _armStraight;

        private Ray _ray;
        private RaycastHit _hit;

        private bool _teleportCooldownActive;

        private void Start()
        {
            kinectTracker.onCreateActor += OnCreateActorHandler;
            kinectTracker.onDestroyActor += OnDestroyActorHandler;

            if (recognition != null)
            {
                recognition.OnKeywordRecognized += TeleportPlayer;
            }

            ReferenceColliderAndJoints();

            _lineRenderer = GetComponent<LineRenderer>();
        }



        private void OnCreateActorHandler(KinectActor kinectActor)
        {
            _kinectSkeleton = kinectActor.transform.gameObject.GetComponent<KinectSkeleton>();
            ReferenceColliderAndJoints();
            if (recognition != null)
            {
                recognition.OnKeywordRecognized += TeleportPlayer;
            }
            _lineRenderer = GetComponent<LineRenderer>();
            Debug.Log("Actor created " + kinectActor.gameObject.name);

        }

        private void OnDestroyActorHandler(KinectActor kinectActor)
        {
            _kinectSkeleton = null;
        }

        private void ReferenceColliderAndJoints()
        {
            if (_kinectSkeleton != null)
            {
                _shoulderJointRight = _kinectSkeleton.GetJoint(Windows.Kinect.JointType.ShoulderRight);
                _elbowJointRight = _kinectSkeleton.GetJoint(Windows.Kinect.JointType.ElbowRight);
                _wristJointRight = _kinectSkeleton.GetJoint(Windows.Kinect.JointType.WristRight);
                _armStraightCollider = _elbowJointRight.GetComponentInChildren<CapsuleCollider>();
                _wristJointLeft = _kinectSkeleton.GetJoint(Windows.Kinect.JointType.WristLeft);
                _leftHandCollider = _wristJointLeft.GetComponent<Collider>();
                _headJoint = _kinectSkeleton.GetJoint(Windows.Kinect.JointType.Head);
            }
        }

        private void TeleportPlayer(UnityEngine.Windows.Speech.PhraseRecognizedEventArgs obj)
        {
            Debug.Log($"Keyword recognized {obj.text}");

            if (!obj.text.Equals("los") && !obj.text.Equals("jump") && !obj.text.Equals("teleport")) return;
            
            Debug.Log($"Keyword recognized {obj.text}");
            if (!_armStraight) return;
            
            Debug.Log("Arm is Straight");
            if (!Physics.Raycast(_ray, out _hit, 10, teleportLayers)) return;
            
            Debug.Log(" -> Teleporting");
            if (!Physics.Raycast(_ray, out _hit, 10, teleportLayers)) return;

            var parent = kinectTracker.gameObject.transform.parent;
            var position = parent.position;
            Vector3 area = position;
            Vector3 player = _kinectSkeleton.gameObject.transform.position;

            Vector3 offset = _hit.point - player;
            Vector3 relative = area - player;
            Vector3 newPosition = relative + offset;

            position += newPosition - relative;
            position = new(position.x, _hit.point.y, position.z);
            parent.position = position;
        }

        IEnumerator ActivateCooldown()
        {
            _teleportCooldownActive = true;
            yield return new WaitForSeconds(2f);
            _teleportCooldownActive = false;
        }

        private void Update()
        {
            Debug.Log("skeleton is " + _kinectSkeleton);
            if (_kinectSkeleton == null)
            {
                _lineRenderer.enabled = false;
                return;
            }

            if (_kinectSkeleton != null && _armStraightCollider != null)
            {
                bool shoulderInside = _armStraightCollider.bounds.Contains(_shoulderJointRight.transform.position);
                bool elbowInside = _armStraightCollider.bounds.Contains(_elbowJointRight.transform.position);
                bool wristInside = _armStraightCollider.bounds.Contains(_wristJointRight.transform.position);

                if (shoulderInside && elbowInside && wristInside)
                {
                    _lineRenderer.enabled = true;
                    // Set the start and end points of the LineRenderer
                    _lineRenderer.SetPosition(0, _shoulderJointRight.transform.position);
                    _lineRenderer.SetPosition(1, _wristJointRight.transform.position);

                    // Create a ray from the shoulder to the wrist direction
                    _ray = new Ray(_shoulderJointRight.transform.position, (_wristJointRight.transform.position - _shoulderJointRight.transform.position).normalized);

                    // Check if the ray hits a collider
                    if (Physics.Raycast(_ray, out _hit, 10, teleportLayers))
                    {
                        // Set the end point of the LineRenderer to the hit point
                        _lineRenderer.SetPosition(1, _hit.point);

                        Debug.Log(_leftHandCollider.bounds.Contains(_headJoint.transform.position));
                        // teleport if hand is on head
                        if(_leftHandCollider.bounds.Contains(_headJoint.transform.position) && _teleportCooldownActive == false)
                        {
                            var parent = kinectTracker.gameObject.transform.parent;
                            var position = parent.position;
                            Vector3 area = position;
                            Vector3 player = _kinectSkeleton.gameObject.transform.position;

                            Vector3 offset = _hit.point - player;
                            Vector3 relative = area - player;
                            Vector3 newPosition = relative + offset;

                            position += newPosition - relative;
                            position = new Vector3(position.x, _hit.point.y, position.z);
                            parent.position = position;
                            StartCoroutine(ActivateCooldown());
                        }
                    }
                    _armStraight = true;
                }
                else
                {
                    _lineRenderer.enabled = false;
                    _armStraight = false;
                }
            }
        }
    }
}