using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using DG.Tweening;
using Scenes;
using UnityEngine;

namespace MAZE
{
    public class ActionMove : ActionMaze
    {
        private bool complete = false;

        public override void OnStart()
        {
            base.OnStart();

            complete = false;

            var scene = AppManager.Instance.CurrScene as SceneMaze;
            var coordinate = mouse.CoordinateNext;
            var dest = scene.mazeGenerator.maze[coordinate.x, coordinate.y].block.transform.position;
            mouse.transform.
                DOMove(dest, 0.1f).
                OnComplete(() =>
                {
                    mouse.Coordinate = mouse.CoordinateNext;
                    complete = true;
                });
        }

        public override TaskStatus OnUpdate()
        {
            if (complete == true)
            {
                return TaskStatus.Success;
            }
            else 
            {
                
            }

            return TaskStatus.Running;            
        }
    }
 
}
