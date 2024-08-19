using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Gostop;
using Scenes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gostop
{
    public class GostopActionOpen1More : GostopAction
    {
        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();
            
            board.PopDeckCard();
        }

        public override TaskStatus OnUpdate()
        {
#if UNITY_EDITOR
            if (UnityEngine.Application.isPlaying == false)
            {
                return TaskStatus.Failure;
            }
#endif

            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();
            int count = 0;
            foreach (var slot in board.bottoms)
            {
                count += slot.Value.MoveCount();
                if (count > 0)
                {
                    break;
                }
            }

            if (count == 0)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
    }
}