using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

namespace Ant
{
    public class MonsterBase : MonoBehaviour
    {
        protected Rigidbody2D rigidBody = null;
        protected SpriteRenderer hand = null;
        protected NavMeshAgent agent = null;
        
        protected MonsterData objData = null;

        protected SkeletonAnimation skel = null;
        protected bool loop = false;

        protected enum SkellAnimationState { die, idle, run, run_shoot };
        protected SkellAnimationState state = SkellAnimationState.idle;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector3 direction)
        {
            if (rigidBody != null)
            {
                hand.transform.position = transform.position + (direction * 0.6f);

                var position = transform.position + (direction * objData.speed);
                rigidBody.MovePosition(position);
            }

            SetState(SkellAnimationState.run);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            SetState(SkellAnimationState.idle);
        }

        /// <summary>
        /// Å¸ÀÏ ±ú±â
        /// </summary>
        public void Hit()
        {
            SetState(SkellAnimationState.run_shoot);
        }
        protected void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            // Play some sound if the event named "footstep" fired.
            //if (e.Data.Name == footstepEventName)
            {
                Debug.Log($"HandleEvent {e.Data.Name}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        protected void SetState(SkellAnimationState s)
        {
            if (s == state)
                return;

            state = s;
            string name = SkellAnimationState.idle.ToString();
            switch (state)
            {
                case SkellAnimationState.idle:
                case SkellAnimationState.run:
                case SkellAnimationState.die:
                    loop = true;
                    break;
                case SkellAnimationState.run_shoot:
                    loop = false;
                    break;
            }

            name = state.ToString();
            skel.state.SetAnimation(0, name, loop);
        }
    }
}
