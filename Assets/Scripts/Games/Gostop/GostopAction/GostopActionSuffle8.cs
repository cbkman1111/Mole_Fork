using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using System.Linq;

namespace Gostop
{
    public class GostopActionSuffle8 : GostopAction
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
     
            int count = 0;
            foreach (var slot in board.bottoms)
            {
                count += slot.Value.MoveCount();
                if(count > 0)
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
            //return TaskStatus.Failure;
        }
    }
}