using Creature;
using UnityEngine;

namespace Giant.Camera
{
    public class CameraControl : MonoBehaviour
    {
        private WorldObject Target = null;

        [SerializeField] private Vector3 offset;
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float smoothDampTime = 0.15f;

        [SerializeField] private Transform boundLeft;
        [SerializeField] private Transform boundRight;
        [SerializeField] private Transform boundTop;
        [SerializeField] private Transform boundBottom;
        
        private Vector3 _minBounds;
        private Vector3 _maxBounds;
        private Vector3 _currentVelocity = Vector3.zero;

        public UnityEngine.Camera Cam { get; private set; }
        public float targetZoom;

        public void SetBounds(UnityEngine.Camera cam, WorldObject target, GameObject[] area)
        {
            Cam = cam;
            Target = target;
            boundLeft = area[0].transform;
            boundTop = area[1].transform;
            boundRight = area[2].transform;
            boundBottom = area[3].transform;

            var desiredPosition = Target.transform.position + (Vector3)offset;
            desiredPosition.z = transform.position.z;
            transform.position = desiredPosition;

            SetBounds();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Cam == null)
                return;

            if (Target == null) 
                return;

            MoveCamera();
        }

        private void Update()
        {
            if (Cam == null)
                return;

            HandleZoom();
        }


        /// <summary>
        /// 줌 처리
        /// Handle zoom functionality
        /// </summary>
        private void HandleZoom()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                var previousZoom = targetZoom;

                targetZoom -= scroll * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

                Cam.orthographicSize = targetZoom;

                if (Mathf.Abs(previousZoom - targetZoom) > 0.01f)
                {
                    var currentPos = transform.position;
                    var verticalExtent = Cam.orthographicSize;
                    var horizontalExtent = verticalExtent * Cam.aspect;

                    var minX = _minBounds.x + horizontalExtent;
                    var maxX = _maxBounds.x - horizontalExtent;
                    var minZ = _minBounds.z + verticalExtent;
                    var maxZ = _maxBounds.z - verticalExtent;

                    if (minX > maxX)
                    {
                        var centerX = (_minBounds.x + _maxBounds.x) * 0.5f;
                        currentPos.x = centerX;
                    }
                    else
                    {
                        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
                    }

                    if (minZ > maxZ)
                    {
                        var centerZ = (_minBounds.z + _maxBounds.z) * 0.5f;
                        currentPos.z = centerZ;
                    }
                    else
                    {
                        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);
                    }

                    transform.position = currentPos;
                    _currentVelocity = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// 카메라 움직임 처리
        /// Handle camera movement
        /// </summary>
        private void MoveCamera()
        {
            var desiredPosition = Target.transform.position + (Vector3)offset;
            desiredPosition.y = transform.position.y;

            var vertExtent = Cam.orthographicSize;
            var horzExtent = vertExtent * Cam.aspect;

            var minX = _minBounds.x + horzExtent;
            var maxX = _maxBounds.x - horzExtent;

            if (minX > maxX)
            {
                var centerX = (_minBounds.x + _maxBounds.x) * 0.5f;
                desiredPosition.x = centerX;

                _currentVelocity.x = 0;
            }
            else
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            }

            var minZ = _minBounds.z + vertExtent;
            var maxZ = _maxBounds.z - vertExtent;

            if (minZ > maxZ)
            {
                var centerZ = (_minBounds.z + _maxBounds.z) * 0.5f;
                desiredPosition.z = centerZ;
                _currentVelocity.z = 0;
            }
            else
            {
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
            }

            var smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothDampTime);
            transform.position = smoothedPosition;
        }

        /// <summary>
        /// 객체에서 카메라 경계 설정
        /// Set camera boundaries from objects
        /// </summary>
        private void SetBounds()
        {
            if (boundLeft && boundRight && boundTop && boundBottom)
            {
                _minBounds = new Vector3(boundLeft.position.x, 0, boundBottom.position.z);
                _maxBounds = new Vector3(boundRight.position.x, 0, boundTop.position.z);
            }
            else
            {
                Debug.LogWarning("카메라 바운드 오브젝트가 설정되지 않았습니다. Camera bounds object is not set.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_minBounds == Vector3.zero && _maxBounds == Vector3.zero)
                return;

            Gizmos.color = Color.green;

            var min = new Vector3(_minBounds.x, 0, _minBounds.z);
            var max = new Vector3(_maxBounds.x, 0, _maxBounds.z);

            var bottomLeft = new Vector3(min.x, 0, min.z);
            var bottomRight = new Vector3(max.x, 0, min.z);
            var topRight = new Vector3(max.x, 0, max.z);
            var topLeft = new Vector3(min.x, 0, max.z);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}
