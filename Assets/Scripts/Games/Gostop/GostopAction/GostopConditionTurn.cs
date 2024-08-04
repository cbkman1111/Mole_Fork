using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Gostop;
using Scenes;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gostop
{
    public class GostopConditionTurn : Conditional
    {
        int jokerCount = 0;

        public override void OnStart()
        {
            /*
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();
            jokerCount = board.CheckJoker();
            */
        }

        public override TaskStatus OnUpdate()
        {
#if UNITY_EDITOR
            if (UnityEngine.Application.isPlaying == false)
            {
                return TaskStatus.Failure;
            }
#endif
            var board = GetComponent<Board>();
            if (board.MyTurn() == true)
            {
                
            }

            return TaskStatus.Success;
        }
    }
}