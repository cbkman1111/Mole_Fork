using System.Collections;
using UnityEngine;

public class SceneLoading : SceneBase
{
    public SceneLoading(SCENES scene) : base(scene)
    {
    }

    public override bool Init()
    {
        UIManager.Instance.OpenMenu<UILoadingMenu>("UILoadingMenu");

        StartCoroutine("Load");
        return true;
    }
    
    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1.0f);
        
        SoundManager.Instance.Load();

        AppManager.Instance.ChangeScene(SCENES.GAME);
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
