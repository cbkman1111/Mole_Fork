using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Behavior.Demo
{
    public class FovMesh : MonoBehaviour, ICharacterStateChannelModifier
    {
        public CharacterStateEventChannel StateChannel { get; set; }

        [SerializeField] private Material m_Material;
        [SerializeField] private float m_Radius = 10;
        [SerializeField] private int m_Segments = 32;
        [SerializeField] private float m_Angle = 45;
        [SerializeField] private float m_Height = 0.5f;
        [SerializeField] private LayerMask m_CollisionLayers;
        
        private MeshFilter m_Filter;
        private MeshRenderer m_Renderer;

        private void Start()
        {
            m_Filter = gameObject.GetOrAddComponent<MeshFilter>();

            m_Renderer = gameObject.GetOrAddComponent<MeshRenderer>();
            m_Renderer.material = m_Material;

            Mesh diskMesh = new Mesh();
            diskMesh.MarkDynamic();
            diskMesh.name = "Disk";
            m_Filter.mesh = diskMesh;

            StateChannel.Event += StateChannel_Event;
        }

        private void OnEnable()
        {
            if (!m_Renderer)
            {
                return;
            }
               
            m_Renderer.enabled = true;
        }

        private void OnDisable()
        {
            if (!m_Renderer)
            {
                return;
            }

            m_Renderer.enabled = false;
        }

        private void StateChannel_Event(CharacterState newState)
        {
            enabled = newState != CharacterState.Dead;
        }

        private void LateUpdate()
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            UpdateRendering();
        }

        private void UpdateRendering()
        {
            Vector3[] vertices = new Vector3[m_Segments + 1];
            int[] triangles = new int[m_Segments * 3];

            vertices[0] = Vector3.zero;
            float angleStep = m_Angle / m_Segments;
            for (int i = 1; i < m_Segments + 1; i++)
            {
                float angleRad = Mathf.Deg2Rad * angleStep * i + (Mathf.PI - Mathf.Deg2Rad * m_Angle) / 2;

                Vector3 directionWorld = transform.TransformDirection(new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)));

                float collisionRadius = m_Radius;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * m_Height, directionWorld, out hit, m_Radius, m_CollisionLayers))
                {
                    collisionRadius = hit.distance;
                }

                vertices[i] = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)) * collisionRadius;
            }

            for (int i = 0; i < m_Segments - 1; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }

            m_Filter.mesh.Clear();
            m_Filter.mesh.vertices = vertices;
            m_Filter.mesh.triangles = triangles;
            m_Filter.mesh.RecalculateNormals();
        }
    }
}