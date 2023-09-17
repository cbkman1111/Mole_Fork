using Ant;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileMap;
using UnityEngine;

public class SceneTileMap : SceneBase
{
    UIMenuTileMap menu = null;

    Pige pigeon = null;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init(JSONObject param)
    {
        menu = UIManager.Instance.OpenMenu<UIMenuTileMap>("UIMenuTileMap");
        if (menu != null)
        {
            menu.InitMenu(
                move: (Vector3 angle) => {
                    OnMove(angle);
                },
                stop: () => {
                    OnStop();
                });
        }

        var prefab = ResourcesManager.Instance.LoadInBuild<Pige>("PigeonTemp");
        if (prefab != null)
        {
            pigeon = GameObject.Instantiate<Pige>(prefab);
            pigeon.transform.position = Vector3.zero;
            //pigeon.transform.rotation = Quaternion.identity;
        }
        /*
        var prefabPlayer = ResourcesManager.Instance.LoadInBuild<Skell.Player>("Player");
        if (prefabPlayer != null)
        {
            var player = GameObject.Instantiate<Skell.Player>(prefabPlayer);
            player.transform.position = new Vector3(2, 0, 0);
        }

        */
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    public void OnMove(Vector3 angle)
    {
        //pigeon.Move(angle);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnStop()
    {
  
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {

    }

    public override void OnTouchBean(Vector3 position)
    {
        menu.Joystick.TouchBegin(position);
    }

    public override void OnTouchEnd(Vector3 position)
    {
        menu.Joystick.TouchEnd(position);

        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            var layer = hit.collider.gameObject.layer;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Card"))
            {
             
            }
        }
    }

    public override void OnTouchMove(Vector3 position)
    {
        menu.Joystick.TouchMove(position);
    }
}