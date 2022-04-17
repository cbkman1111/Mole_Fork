using Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoSingleton<AppManager>
{
    private Dictionary<string, SceneBase> scenes = null;
    public string SceneName { get; set; }

    public override bool Init()
    {
        SceneName = "";

        scenes = new Dictionary<string, SceneBase>();
        scenes.Add("SceneIntro", new IntroScene(SceneBase.SCENES.INTRO));
        scenes.Add("SceneLoading", new LoadingScene(SceneBase.SCENES.LOADING));
        scenes.Add("SceneGame", new GameScene(SceneBase.SCENES.GAME));

        SceneManager.sceneUnloaded += OnUnLoadScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    private void OnUnLoadScene(Scene scene)
    {
        Debug.Log($"{TAG} UnLoadScene {scene.name}");
    }

    private IEnumerator LoadScene(string name)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            yield return null;

            if (async.progress < 0.9f)
            {
                if (async.progress == 1)
                {
                    async.allowSceneActivation = true;
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                async.allowSceneActivation = true;
                yield break;
            }
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scenes.TryGetValue(scene.name, out SceneBase info) == true)
        {
            bool ret = info.Init();
            if (ret == true)
            {
                Debug.Log($"OnSceneLoaded. {scene.name}");
            }
            else
            { 
                Debug.LogError($"{TAG} init is failed. {scene.name}");
            }
        }
        else
        {
            Debug.LogError($"{TAG} not exist. {scene.name}");
        }
    }

    public void ChangeScene(SceneBase.SCENES scene)
    {
        var info = scenes.Where(e => e.Value.scene == scene).First();
        var name = info.Key;

        if (SceneName.CompareTo(name) == 0)
        {
            Debug.LogWarning($"{TAG} Same Scene. {name}, {info.Value}");
            return;
        }

        SceneName = name;
        Debug.Log($"{TAG} ChangeScene. {name}, {info.Value}");

        StartCoroutine("LoadScene", name);
    }

    public void OnAppPause(bool paused)
    {
        Debug.Log($"{TAG} OnAppPause. {paused}");
    }

    public void OnAppFocus(bool focus)
    {
        Debug.Log($"{TAG} OnAppFocus. {focus}");
    }

    public  void OnAppQuit()
    {
        Debug.Log($"{TAG} OnAppQuit.");
    }

}


