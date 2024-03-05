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
    public class GostopActionOpen8 : GostopAction
    {
        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            board.Shuffle8Card();
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

            return TaskStatus.Success;
        }
    }
}