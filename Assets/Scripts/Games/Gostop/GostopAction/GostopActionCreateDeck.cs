using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using System.Linq;


namespace Gostop
{
    public class GostopActionCreateDeck : GostopAction
    {
        public override void OnStart()
        {
            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();
            board.CreateDeck();
        }

        public override TaskStatus OnUpdate()
        {
#if UNITY_EDITOR
            if(UnityEngine.Application.isPlaying == false)
            {
                return TaskStatus.Failure;
            }
#endif

            var scene = AppManager.Instance.CurrScene as SceneGostop;
            var board = GetComponent<Board>();
            int count = board.deck.Where(card => card.ListTween.Count != 0).ToList().Count;
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