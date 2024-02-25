using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using UnityEngine;

namespace Gostop
{
    public class GostopAction : Action
    {
        //public SharedTransform sharedMouse = null;
        //protected MazeMouse mouse = null;

        public override void OnAwake()
        {
            base.OnAwake();

            //mouse = (sharedMouse.GetValue() as Transform).GetComponent<MazeMouse>();
        }
    }
}