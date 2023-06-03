using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class MonsterBase : MonoBehaviour
    {
        protected Rigidbody2D rigidBody = null;
        protected SpriteRenderer hand = null;
        protected NavMeshAgent agent = null;
        
        protected MonsterData objData = null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Create<T>(MonsterData data, bool enableAgent = true) where T : MonsterBase
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Monster");
            var obj = Instantiate<GameObject>(prefab);
            if (obj == null)
            {
                return default(T);
            }

            var monster = obj.AddComponent<T>();
            if (monster != null && monster.Init(data, enableAgent) == true)
            {
                return monster;
            }

            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="enableAgent"></param>
        /// <returns></returns>
        protected bool Init(MonsterData data, bool enableAgent = true)
        {
            rigidBody = GetComponent<Rigidbody2D>();
            if (rigidBody == null)
            {
                Debug.LogError($"rigidBody is null.");
                return false;
            }

            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError($"agent is null.");
                return false;
            }

            if(LoadSprite() == false)
            {
                Debug.LogError($"LoadSprite() false.");
                return false;
            }

            objData = data;
            agent.enabled = enableAgent;
            agent.speed = objData.speed;
            transform.position = data.position;

            hand = transform.Find("Hand").GetComponent<SpriteRenderer>();
            return true;
        }

        protected virtual bool LoadSprite()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        public void SetDestination(Vector3 dest)
        {
            agent.SetDestination(dest);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnArrive()
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetHandPosition()
        {
            return hand.transform.position;
        }

        public void UpdateData()
        {
            objData.position = transform.position;
        }
    }
}
