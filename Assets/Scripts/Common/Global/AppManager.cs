using Singleton;
using System;
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
    public JSONObject Param { get; set; }

    private Vector3 begin = Vector3.zero;
    private Vector3 curr = Vector3.zero;

    public override bool Init()
    {
        scenes = new Dictionary<string, SceneBase.SCENES>();
        scenes.Add("SceneIntro", SceneBase.SCENES.INTRO);
        scenes.Add("SceneMenu", SceneBase.SCENES.MENU);
        scenes.Add("SceneLoading", SceneBase.SCENES.LOADING);
        scenes.Add("SceneGame", SceneBase.SCENES.GAME);
        scenes.Add("SceneGostop", SceneBase.SCENES.GAME_GOSTOP);
        scenes.Add("SceneTileMap", SceneBase.SCENES.GAME_TILEMAP);
        scenes.Add("SceneAntHouse", SceneBase.SCENES.GAME_ANTHOUSE);

        SceneManager.sceneLoaded += OnSceneLoaded;

        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    private IEnumerator LoadScene(string name, bool loading)
    {
        if (CurrScene != null) // 동일씬 로드.
        {
            CurrScene.UnLoaded();
        }

        if (loading == true)
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SceneLoading", LoadSceneMode.Single);
            async.allowSceneActivation = true;
            
            float percent = 0;
            while (async.isDone == false)
            {
                percent = Mathf.Clamp01(async.progress / 0.9f);
                if (percent == 1.0)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }

            AsyncOperation asyncNext = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            asyncNext.allowSceneActivation = false;
            while (asyncNext.isDone == false)
            {
                percent = Mathf.Clamp01(asyncNext.progress / 0.9f);
                
                var currScene = GetCurrentScene() as SceneLoading;
                if (currScene == true)
                {
                    currScene.SetPercent(percent);
                }
                
                yield return new WaitForSeconds(0.1f);

                if (percent == 1.0)
                {
                    asyncNext.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        else 
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            float percent = 0;
            while (async.isDone == false)
            {
                percent = Mathf.Clamp01(async.progress / 0.9f);
                if (percent == 1)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        

        Debug.Log($"{TAG} Scene Load Complete");
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
        switch (mode)
        {
            case LoadSceneMode.Additive:
            case LoadSceneMode.Single:
                if (scenes.TryGetValue(name, out SceneBase.SCENES index) == true)
                {

                    // UI 제거.
                    UIManager.Instance.Clear();

                    // 씬 스크립트 부착.
                    switch (index)
                    {
                        case SceneBase.SCENES.INTRO:
                            CurrScene = new GameObject(name).AddComponent<SceneIntro>();
                            break;
                        case SceneBase.SCENES.MENU:
                            CurrScene = new GameObject(name).AddComponent<SceneMenu>();
                            break;
                        case SceneBase.SCENES.LOADING:
                            CurrScene = new GameObject(name).AddComponent<SceneLoading>();
                            break;
                        case SceneBase.SCENES.GAME:
                            CurrScene = new GameObject(name).AddComponent<SceneGame>();
                            break;
                        case SceneBase.SCENES.GAME_GOSTOP:
                            CurrScene = new GameObject(name).AddComponent<SceneGostop>();
                            break;
                        case SceneBase.SCENES.GAME_TILEMAP:
                            CurrScene = new GameObject(name).AddComponent<SceneTileMap>();
                            break;
                        case SceneBase.SCENES.GAME_ANTHOUSE:
                            CurrScene = new GameObject(name).AddComponent<SceneAntHouse>();
                            break;
                    }

                    // 카메라를 미리 셋 초기화 전에 사용할 수 있도록.
                    CurrScene.MainCamera = Camera.main;

                    // 초기화.
                    CurrScene.Init(Param);
                }
                break;

            default:
                Debug.LogWarning($"{TAG} mode not exist. {name}/{mode}");
                break;
        }
    }

    public void ChangeScene(SceneBase.SCENES scene, bool loading = true, JSONObject param = null)
    {
        var info = scenes.Where(e => e.Value == scene).First();
        var name = info.Key;
        
        Param = param; // 파라미터 설정.

        if (CurrScene != null && CurrScene.Scene == info.Value)
        {
            Debug.LogWarning($"{TAG} Same Scene.");
            return;
        }

        StartCoroutine(LoadScene(name, loading));
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
        else if (Input.GetKey(KeyCode.A))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnMove(Vector3.left);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnMove(Vector3.right);
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnMove(Vector3.up);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnMove(Vector3.down);
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnStop();
            }
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnStop();
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnStop();
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            SceneAntHouse scene = CurrScene as SceneAntHouse;
            if (scene != null)
            {
                scene.OnStop();
            }
        }


        if (phase >= 0)
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


