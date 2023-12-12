using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using UnityEngine;

namespace MAZE
{
    public class ActionArrive : ActionMaze
    {


        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneMaze;

            scene.OnGameOver();
        }

        public override TaskStatus OnUpdate()
        {
            //return base.OnUpdate();

            return TaskStatus.Success; 
        }
    }
}