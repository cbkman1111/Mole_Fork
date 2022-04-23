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
        
    }

    public override void OnTouchMove(Vector3 position)
    {
        
    }
}
