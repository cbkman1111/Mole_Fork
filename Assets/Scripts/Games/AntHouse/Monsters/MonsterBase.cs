using Common.Global;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using TileMap;
using DG.Tweening;

namespace Ant
{
    public class MonsterBase : MonoBehaviour
    {
        protected StateMachine<ObjectState> _stateMachine = new StateMachine<ObjectState>();

        protected Rigidbody2D rigidBody = null;
        protected SpriteRenderer hand = null;
        protected NavMeshAgent agent = null;
        protected MonsterData objData = null;

        [SerializeField] 
        protected SkeletonAnimation _skel = null;
        protected bool loop = false;

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
            _stateMachine.OnEnterState = OnEnterState;
            _stateMachine.OnPopState = OnPopState;
            _stateMachine.PushState(ObjectState.Idle);
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


        public virtual void Move(Vector3 angle)
        {
            if (rigidBody != null)
            {
                hand.transform.position = transform.position + (angle * 0.6f);
                var position = transform.position + (angle * objData.speed);
                rigidBody.MovePosition(position);
            }

            switch (_stateMachine.State)
            {
                //case ObjectState.None:
                case ObjectState.Idle:
                    if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x))
                    {
                        if (angle.z > 0f)
                            _stateMachine.PushState(ObjectState.MoveUp);
                        else if (angle.z < 0f)
                            _stateMachine.PushState(ObjectState.MoveDown);
                    }
                    else
                    {
                        if (angle.x > 0f)
                            _stateMachine.PushState(ObjectState.MoveLeft);
                        else
                            _stateMachine.PushState(ObjectState.MoveRight);
                    }

                    break;
                case ObjectState.MoveLeft:
                case ObjectState.MoveRight:
                case ObjectState.MoveUp:
                case ObjectState.MoveDown:

                    if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x))
                    {
                        if (angle.z > 0f)
                            _stateMachine.PushState(ObjectState.MoveUp);
                        else if (angle.z < 0f)
                            _stateMachine.PushState(ObjectState.MoveDown);
                    }
                    else
                    {
                        if (angle.x > 0f)
                            _stateMachine.PushState(ObjectState.MoveLeft);
                        else
                            _stateMachine.PushState(ObjectState.MoveRight);
                    }

                    /*
                    var speed = 0.10f;
                    var target = transform.position + (angle * speed);
                    target.y = 0.5f;
                    transform.DOKill();
                    transform.DOMove(target, 0.1f).
                        OnUpdate(() =>
                        {
                        }).
                        SetEase(Ease.Linear).
                        OnComplete(() =>
                        {
                            _stateMachine.PushState(ObjectState.Stop);
                        });
                    */
                    break;

                case ObjectState.Stop:

                    break;
                case ObjectState.Click:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            SetState(ObjectState.Idle);
        }

        /// <summary>
        /// 타일 깨기
        /// </summary>
        public void Hit()
        {
            SetState(ObjectState.Attack);
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
        protected void SetState(ObjectState s)
        {
            _stateMachine.PushState(s);
        }

        protected virtual void OnPopState(ObjectState state)
        {
        }

        protected virtual void OnEnterState(ObjectState before, ObjectState after)
        {
            switch (after)
            {
                case ObjectState.Idle:
                    _skel.state.SetAnimation(0, "movement/idle-front", true);
                    break;
                case ObjectState.MoveLeft:
                    _skel.state.SetAnimation(0, "movement/trot-left", true);
                    break;
                case ObjectState.MoveRight:
                    _skel.state.SetAnimation(0, "movement/trot-right", true);
                    break;
                case ObjectState.MoveUp:
                    _skel.state.SetAnimation(0, "movement/trot-back", true);
                    break;
                case ObjectState.MoveDown:
                    _skel.state.SetAnimation(0, "movement/trot-front", true);
                    break;

                case ObjectState.Stop:
                    //_skel.state.SetAnimation(0, "idle", true);
                    _skel.state.SetAnimation(0, "emotes/sulk", false);
                    _stateMachine.PushState(ObjectState.Idle);
                    break;
                case ObjectState.Click:
                    if (after == ObjectState.Click)
                    {
                        transform.DOPunchScale(transform.forward, 0.1f).OnComplete(() => {
                            SetState(ObjectState.Idle);
                        });
                    }

                    break;
            }
        }
    }
}
