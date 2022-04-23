using UnityEngine;

public class SceneGame : SceneBase
{
    public SceneGame(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        SoundManager.Instance.PlayMusic("17856_1462216818");
        
        UIGameMenu menu = UIManager.Instance.OpenMenu<UIGameMenu>("UIGameMenu");
        if(menu != null)
        {
            menu.InitMenu();
        }

        return true;
    }

    public override void OnTouchBean(Vector3 position)
    {
        
    }

    public override void OnTouchEnd(Vector3 position)
    {
        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
            Debug.Log(hit.point);
        }
    }

    public override void OnTouchMove(Vector3 position)
    {
        
    }
}
