using Common.Global;
using Common.Scene;
using UnityEngine;

public class AppDelegate : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void main()
    {
        Debug.Log("App Started. by AppDelegate.cs");
        
        Application.targetFrameRate = 100;

        //SoundManager.Instance.Load();

        AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneIntro, false);
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
