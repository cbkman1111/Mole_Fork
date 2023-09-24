using System.Collections;
using System.Collections.Generic;
using Common.Global;
using DG.Tweening;
using Games.TileMap.Datas;
using Scenes;
using UnityEngine;

namespace TileMap
{
    public class Animal : Creature
    {
        private bool idle = true;
        
        public virtual void Move(Vector3 angle)
        {
            if (idle == true)
            {
                idle = false;
                skel.state.SetAnimation(0, "run", true);
            }

            var speed = 0.25f;
            var target = transform.position + (angle * speed);
            target.y = 0.5f;

            /*
            if (AppManager.Instance.CurrScene is not SceneTileMap scene)
                return;
            
            var targetX = (int)target.x;
            var targetZ = (int)target.z;
            if (targetX < 0 || targetX > scene.MapData.Width)
                return;
            if (targetZ < 0 || targetZ > scene.MapData.Height)
                return;
            
            var adress = targetX + targetZ * scene.MapData.Width;
            var data = scene.MapData.Data[adress];
            if (data.Tile.type == TileType.Ground)
            */
            {
                transform.DOKill();
                transform.DOMove(target, 0.1f).
                    OnUpdate(() =>
                    {
                    }).
                    SetEase(Ease.Linear).
                    OnComplete(() =>
                    {
                        idle = true;
                        skel.state.SetAnimation(0, "idle", true);
                    });
            }
        }
    }
}
