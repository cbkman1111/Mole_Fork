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
        
        // 번들 데이터 로드.
        //ResourcesManager.Instance.Load();
        // 사운드 로드.
        SoundManager.Instance.Load();

        // 
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
