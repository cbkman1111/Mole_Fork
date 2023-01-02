using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Monster : MonoBehaviour
    {
        protected Rigidbody2D rigidBody = null;
        protected SpriteRenderer hand = null;
        protected NavMeshAgent agent = null;
        
        private ObjectData objData = null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Create<T>(GameObject prefab, ObjectData data, bool enableAgent = true) where T : Monster
        {
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
        protected bool Init(ObjectData data, bool enableAgent = true)
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

            objData = data;
            agent.enabled = enableAgent;
            transform.position = data.position;

            hand = transform.Find("Hand").GetComponent<SpriteRenderer>();
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
        /// <param name="direction"></param>
        public void Move(Vector3 direction)
        {
            if(rigidBody != null)
            {
                var position = transform.position + direction * objData.speed;
                rigidBody.MovePosition(position);
                hand.transform.position = transform.position + (direction * 0.6f);
            }
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
