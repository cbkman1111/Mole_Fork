using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneTileMap : SceneBase
{
    public SceneTileMap(SCENES scene) : base(scene)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init()
    {
        UIMenuTileMap menu = UIManager.Instance.OpenMenu<UIMenuTileMap>("UI/UIMenuTileMap");
        if (menu != null)
        {
            menu.InitMenu();
        }

        var prefabPigeon = ResourcesManager.Instance.LoadBundle<Pigeon>("Pigeon.prefab");
        var pigeon = Instantiate<Pigeon>(prefabPigeon);
        pigeon.transform.position = Vector3.zero;
        pigeon.transform.rotation = Quaternion.identity;

        var prefabPlayer = ResourcesManager.Instance.LoadBundle<Skell.Player>("Player.prefab");
        var player = Instantiate<Skell.Player>(prefabPlayer);
        player.transform.position = new Vector3(2,0,0);

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {

    }

    public override void OnTouchBean(Vector3 position)
    {

    }

    public override void OnTouchEnd(Vector3 position)
    {
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

    }
}