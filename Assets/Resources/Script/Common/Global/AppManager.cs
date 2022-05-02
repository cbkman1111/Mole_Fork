using Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AppManager : MonoSingleton<AppManager>
{
    private Dictionary<string, SceneBase.SCENES> scenes = null;
    public SceneBase CurrScene { get; set; }

    private Vector3 begin = Vector3.zero;
    private Vector3 curr = Vector3.zero;

    public override bool Init()
    {
        scenes = new Dictionary<string, SceneBase.SCENES>();
        scenes.Add("SceneIntro", SceneBase.SCENES.INTRO);
        scenes.Add("SceneLoading", SceneBase.SCENES.LOADING);
        scenes.Add("SceneGame", SceneBase.SCENES.GAME);

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 번들 데이터 로드.
        ResourcesManager.Instance.Load();
        
        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    private IEnumerator LoadScene(string name)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        float percent = 0;
        while (async.isDone == false)
        {
            percent = async.progress * 100f;
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            else
            {
                if (async.progress == 1)
                {
                    async.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        Debug.Log($"{TAG} Scene Load Complete");
    }


    public void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode mode)
    {
        switch(mode)
        {
            case LoadSceneMode.Additive:
            case LoadSceneMode.Single:
                if (scenes.TryGetValue(sceneLoaded.name, out SceneBase.SCENES idx) == true)
                {
                    if (CurrScene != null && CurrScene.Scene == idx) // 동일씬 로드.
                    {
                        return;
                    }

                    string name = sceneLoaded.name;

                    // UI 제거.
                    UIManager.Instance.Clear();

                    // 씬 추가.
                    if(idx == SceneBase.SCENES.INTRO)
                        CurrScene = new GameObject(name).AddComponent<SceneIntro>();
                    else if (idx == SceneBase.SCENES.LOADING)
                        CurrScene = new GameObject(name).AddComponent<SceneLoading>();
                    else if (idx == SceneBase.SCENES.GAME)
                        CurrScene = new GameObject(name).AddComponent<SceneGame>();

                    // 카메라를 미리 셋 초기화 전에 사용할 수 있도록.
                    CurrScene.MainCamera = Camera.main;

                    // 초기화.
                    CurrScene.Init();
                }
                break;

            default:
                Debug.LogWarning($"{TAG} mode not exist. {sceneLoaded.name}/{mode}");
                break;
        }
    }

    public void ChangeScene(SceneBase.SCENES scene)
    {
        var info = scenes.Where(e => e.Value == scene).First();
        var name = info.Key;

        if (CurrScene != null && CurrScene.Scene == info.Value)
        {
            Debug.LogWarning($"{TAG} Same Scene.");
            return;
        }

        StartCoroutine("LoadScene", name);
        Debug.Log($"{TAG} ChangeScene. {name}, {info.Value}");
    }

    public SceneBase GetCurrentScene()
    {
        return CurrScene;
    }

    public void OnAppPause(bool paused)
    {
        Debug.Log($"{TAG} OnAppPause. {paused}");
    }

    public void OnAppFocus(bool focus)
    {
        Debug.Log($"{TAG} OnAppFocus. {focus}");
    }

    public void OnAppQuit()
    {
        Debug.Log($"{TAG} OnAppQuit.");
    }

    void Update()
    {
        var touches = Input.touches;
        int phase = -1;

        if (touches.Count() == 1)
        {
            phase = (int)touches[0].phase;
            switch ((TouchPhase)phase)
            {
                case TouchPhase.Began:
                    begin = touches[0].position;
                    curr = touches[0].position;
                    break;

                case TouchPhase.Moved:
                    begin = touches[0].position;
                    curr = touches[0].position;
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                    begin = touches[0].position;
                    curr = touches[0].position;
                    break;

                case TouchPhase.Canceled:
                    begin = Vector3.zero;
                    curr = Vector3.zero;
                    break;
            }
        }
        else if (touches.Count() == 2)
        {
            Debug.Log($"{TAG} TOUCH 0 - {touches[0].fingerId}");
            Debug.Log($"{TAG} TOUCH 1 - {touches[1].fingerId}");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            phase = (int)TouchPhase.Began;
            begin = Input.mousePosition;
            curr = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            phase = (int)TouchPhase.Moved;
            curr = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            phase = (int)TouchPhase.Ended;
            curr = Input.mousePosition;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackKeyDown();
        }

        if (phase > 0)
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                switch ((TouchPhase)phase)
                {
                    case TouchPhase.Began:
                        OnTouchBean(curr);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMove(curr);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnd(curr);
                        break;
                }
            }
        }
    }

    void OnTouchBean(Vector3 position)
    {
        if (CurrScene != null)
        {
            CurrScene.OnTouchBean(position);
        }
    }

    void OnTouchMove(Vector3 position)
    {
        if (CurrScene != null)
        {
            CurrScene.OnTouchMove(position);
        }
    }

    void OnTouchEnd(Vector3 position)
    {
        if (CurrScene != null)
        {
            CurrScene.OnTouchEnd(position);
        }
    }

    void BackKeyDown()
    {
        UIManager.Instance.BackKey();
    }
}


