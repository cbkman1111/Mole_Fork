using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Gostop;
using Scenes;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gostop
{
    public class GostopActionCheckHandEmpty : Conditional
    {
        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            board.HandSort();
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

            int count = board.hands[0].Count + board.hands[1].Count;
            if (count > 0)
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