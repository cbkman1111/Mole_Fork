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
    public class GostopActionCheckJoker : Conditional
    {
        int jokerCount = 0;

        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            jokerCount = board.CheckJoker();
        }

        public override TaskStatus OnUpdate()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            int count = 0;
            foreach (var slot in board.bottoms)
            {
                count += slot.Value.Where(card => card.ListTween.Count != 0).ToList().Count;
                if (count > 0)
                {
                    break;
                }
            }

            if (count == 0)
            {
                if (jokerCount == 0)
                {
                    return TaskStatus.Failure;
                    
                }
                else
                {
                    return TaskStatus.Success;
                }
            }
            else
            {
                return TaskStatus.Running;
            }
        }
    }
}