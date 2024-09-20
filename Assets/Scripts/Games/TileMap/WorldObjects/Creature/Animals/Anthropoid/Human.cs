using DG.Tweening;
using UnityEngine;

namespace Creature
{
    public class Human : Animal
    {
        private void UpdateStateAnimation()
        {
            switch (State)
            {
                case ObjectState.Stop:
                case ObjectState.Idle:
                    if (direct == Direct.Left)
                    {
                        Play("movement/idle-left", true);
                    }
                    else if (direct == Direct.Right)
                    {
                        Play("movement/idle-right", true);
                    }
                    else if (direct == Direct.Up)
                    {
                        Play("movement/idle-back", true);
                    }
                    else if (direct == Direct.Down)
                    {
                        Play("movement/idle-front", true);
                    }
                    break;

                case ObjectState.Move:
                    if (direct == Direct.Left)
                    {
                        Play("movement/trot-left", true);
                    }
                    else if (direct == Direct.Right)
                    {
                        Play("movement/trot-right", true);
                    }
                    else if (direct == Direct.Up)
                    {
                        Play("movement/trot-back", true);
                    }
                    else if (direct == Direct.Down)
                    {
                        Play("movement/trot-front", true);
                    }
                    break;
            }
        }

        /// <summary>
        /// 다른 상태로 바뀜.
        /// </summary>
        /// <param name="state"></param>
        public override void OnEnterState(ObjectState state)
        {
            UpdateStateAnimation();

            switch (state)
            {
                case ObjectState.Idle:
                case ObjectState.Move:
                    break;

                case ObjectState.Stop:
                    break;

                case ObjectState.Click:
                    transform.DOPunchScale(transform.forward, 0.1f).
                        OnComplete(() => {
                            ChangeState(ObjectState.Idle);
                        });
                    break;

                case ObjectState.Dead:
                    break;
            }
        }

        protected override void OnDirectChanged(Direct direct)
        {
            UpdateStateAnimation();
        }
        
        protected override void OnDash(Vector3 angle)
        {
            Play("emotes/shrug"); // 대시 표정.
        }
    }
}