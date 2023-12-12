using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using UnityEngine;

namespace MAZE
{
    public class ActionMaze : Action
    {
        public SharedTransform sharedMouse = null;
        
        protected MazeMouse mouse = null;

        public override void OnAwake()
        {
            base.OnAwake();
            mouse = (sharedMouse.GetValue() as Transform).GetComponent<MazeMouse>();
        }
    }
}