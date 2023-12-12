using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using UnityEngine;

namespace MAZE
{
    public class ActionAppear : ActionMaze
    {
     

        public override void OnStart()
        {
            Debug.Log("??");
        
        }

        public override TaskStatus OnUpdate()
        {
            //return base.OnUpdate();

            return TaskStatus.Success; 
        }
    }
}