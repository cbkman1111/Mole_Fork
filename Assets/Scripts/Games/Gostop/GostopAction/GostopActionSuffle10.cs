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
    public class GostopActionSuffle10 : GostopAction
    {
        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            board.Shuffle10Card();

        }

        public override TaskStatus OnUpdate()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();

            int count = 0;
            count += board.hands[0].Where(card => card.ListTween.Count != 0).ToList().Count;
            count += board.hands[1].Where(card => card.ListTween.Count != 0).ToList().Count;

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