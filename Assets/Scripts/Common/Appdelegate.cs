using UnityEngine;

public class AppDelegate : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void main()
    {
        Debug.Log("App Started. by AppDelegate.cs");
        
        SoundManager.Instance.Load();

        AppManager.Instance.ChangeScene(SceneBase.SCENES.Intro, false);
    }

    void OnApplicationPause(bool paused)
    {
        AppManager.Instance.OnAppPause(paused);
    }

    void OnApplicationFocus(bool focus)
    {
        AppManager.Instance.OnAppFocus(focus);
    }

    private void OnApplicationQuit()
    {
        AppManager.Instance.OnAppQuit();
    }
}
