using UnityEngine;

namespace Unity.Behavior.Demo
{
    public class TriggerBehaviorSpawner : MonoBehaviourPool<TriggerBehavior>
    {
        public bool canSpawn { get; set; } = true;
        public float spawnIntervalSpeedMultiplier { get; set; } = 1f;

        [Header("Spawner")]
        [SerializeField] private TriggerBehavior m_Prefab;

        [SerializeField] private float m_SpawnInterval = 1f;
        [SerializeField] private float m_SpawnHeight = 10f;
        [SerializeField] private Vector2 m_SpawnAreaSize = new Vector2(10f, 10f);

        private float m_SpawnTimer;

        public void Spawn(Vector3 origin, int amount, float radius)
        {
            for (int i = 0; i < amount; ++i)
            {
                var randomXZ = Random.insideUnitCircle * radius;
                var offset = new Vector3(randomXZ.x, 0f, randomXZ.y);
                var product = Get();
                product.body.position = origin + offset;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            canSpawn = true;
        }

        protected override TriggerBehavior OnProductCreation()
        {
            return Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
        }

        protected override void OnProductReleased(TriggerBehavior product)
        {
            product.onTrigger -= ReleaseHazard;
            product.gameObject.SetActive(false);
        }

        protected override void OnGetFromPool(TriggerBehavior product)
        {
            product.onTrigger += ReleaseHazard;
            product.gameObject.SetActive(true);
        }

        protected override void OnProductDestruction(TriggerBehavior product)
        {
            Destroy(product);
        }

        private void Update()
        {
            if (!canSpawn)
                return;

            m_SpawnTimer += Time.deltaTime * spawnIntervalSpeedMultiplier;
            if (m_SpawnTimer >= m_SpawnInterval)
            {
                m_SpawnTimer = 0;
                SpawnHazard();
            }
        }

        private void SpawnHazard()
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-m_SpawnAreaSize.x / 2, m_SpawnAreaSize.x / 2),
                m_SpawnHeight,
                Random.Range(-m_SpawnAreaSize.y / 2, m_SpawnAreaSize.y / 2)
            );

            var hazard = Get();
            hazard.transform.position = spawnPosition;
        }

        private void ReleaseHazard(TriggerBehavior hazard)
        {
            Release(hazard);
        }

        private void OnEnable()
        {
            canSpawn = true;
            m_SpawnTimer = 0;
        }

        private void OnDisable()
        {
            canSpawn = false;
        }
    }
}