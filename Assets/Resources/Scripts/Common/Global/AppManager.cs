using Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoSingleton<AppManager>
{
    enum TOUCH_STATE
    {
        NONE,

        BEGIN,
        MOVE,
        END,
    }

    private Dictionary<string, SceneBase> scenes = null;
    public string SceneName { get; set; }
    public SceneBase currScene = null;

    public override bool Init()
    {
        var mainCamera = CameraManager.Instance.MainCamera; // 참조만.

        SceneName = "";

        scenes = new Dictionary<string, SceneBase>();
        scenes.Add("SceneIntro", new SceneIntro(SceneBase.SCENES.INTRO));
        scenes.Add("SceneLoading", new SceneLoading(SceneBase.SCENES.LOADING));
        scenes.Add("SceneGame", new SceneGame(SceneBase.SCENES.GAME));

        //SceneManager.sceneUnloaded += OnUnLoadScene;
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

    public void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode mode)
    {
        // UI를 초기화 합니다.
        UIManager.Instance.RemoveAll();

        // 로드된 씬을 초기화 하빈다.
        if (scenes.TryGetValue(sceneLoaded.name, out SceneBase scene) == true)
        {
            bool ret = scene.Init();
            if (ret == true)
            {
                currScene = scene;
                Debug.Log($"OnSceneLoaded. {sceneLoaded.name}");
            }
            else
            { 
                Debug.LogError($"{TAG} init is failed. {sceneLoaded.name}");
            }
        }
        else
        {
            Debug.LogError($"{TAG} not exist. {sceneLoaded.name}");
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

    public SceneBase GetCurrentScene()
    {
        if (scenes.TryGetValue(SceneName, out SceneBase scene) == true)
        {
            return scene;
        }

        return null;
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

    void Update()
    {
        var touches = Input.touches;
        TOUCH_STATE state = TOUCH_STATE.NONE;

        if (touches.Count() == 1)
        {
            switch (touches[0].phase)
            {
                case TouchPhase.Began:
                    state = TOUCH_STATE.BEGIN;
                    break;

                case TouchPhase.Moved:
                    state = TOUCH_STATE.MOVE;
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    state = TOUCH_STATE.END;
                    break;
            }

            Debug.Log($"TOUCH - {touches[0].fingerId}");
        }
        else if (touches.Count() == 2)
        {
            Debug.Log($"TOUCH 0 - {touches[0].fingerId}");
            Debug.Log($"TOUCH 1 - {touches[1].fingerId}");
        }
        else if(Input.GetMouseButton(0))
        {
            state = TOUCH_STATE.BEGIN;
            Debug.Log($"TOUCH - {Input.mousePosition}");
        }

        switch(state)
        {
            case TOUCH_STATE.BEGIN:
                OnTouchBean(Vector3.zero);
                break;
            case TOUCH_STATE.MOVE:
                OnTouchMove(Vector3.zero);
                break;
            case TOUCH_STATE.END:
                OnTouchEnd(Vector3.zero);
                break;
        }
    }

    void OnTouchBean(Vector3 position)
    {
        if (currScene != null)
        {
            currScene.OnTouchBean(position);
        }
    }

    void OnTouchMove(Vector3 position)
    {
        if (currScene != null)
        {
            currScene.OnTouchMove(position);
        }
    }

    void OnTouchEnd(Vector3 position)
    {
        if (currScene != null)
        {
            currScene.OnTouchEnd(position);
        }
    }
}


