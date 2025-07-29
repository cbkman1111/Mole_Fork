using UnityEngine;

namespace Unity.Behavior.Demo
{
    public class BehaviorAgentFactory : MonoBehaviourPool<BehaviorGraphAgent>
    {
        [Header("Behavior Agent Factory")]
        [SerializeField] private BehaviorGraphAgent m_BehaviorAgentPrefab;

        protected override BehaviorGraphAgent OnProductCreation()
        {
            if (m_BehaviorAgentPrefab == null)
            {
                Debug.LogError("No BehaviorAgent prefab is assigned to the factory.", this);
                return null;
            }

            var gao = GameObject.Instantiate(m_BehaviorAgentPrefab.gameObject, Vector3.zero, Quaternion.identity);
            return gao.GetComponent<BehaviorGraphAgent>();
        }

        protected override void OnProductReleased(BehaviorGraphAgent product)
        {
            product.End();
            product.gameObject.SetActive(false);
        }

        protected override void OnGetFromPool(BehaviorGraphAgent product)
        {
            product.Restart();
            product.gameObject.SetActive(true);
        }

        protected override void OnProductDestruction(BehaviorGraphAgent product)
        {
            Destroy(product);
        }
    }
}