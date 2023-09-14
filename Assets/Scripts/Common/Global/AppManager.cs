using Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AppManager : MonoSingleton<AppManager>
{
    private Dictionary<string, SceneBase> scenes = null;

    //private Stack<SceneBase> stack = null;

    public SceneBase CurrScene { get; set; } = null;
    public JSONObject Param { get; set; } = null;

    private Vector3 begin = Vector3.zero;
    private Vector3 curr = Vector3.zero;

    private LoadState State = LoadState.None;
    public enum LoadState
    {
        None,

        LoadingSceneLoad,
        NextSceneLoad,
        StartLoadPreData,
        WaitComplete,
        LoadComplete,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init()
    {
        scenes = new Dictionary<string, SceneBase>();

        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    /// <returns></returns>
    T StringToEnum<T>(string e)
    {
        return (T)Enum.Parse(typeof(T), e);
    }

    private IEnumerator LoadScene(string name, bool loading)
    {
        if (CurrScene != null) 
        {
            CurrScene.UnLoad();
        }

        UIManager.Instance.Clear();
        CurrScene = null;
        if (loading == true)
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SceneLoading", LoadSceneMode.Single);
            async.allowSceneActivation = true;
            float percent = 0;

            //SceneLoading sceneLoading = new SceneLoading();
            while (async.isDone == false || async.progress < 1.0f)
            {
                yield return null;
            }

            SceneLoading sceneLoading = GetSceneLoading("SceneLoading");
            AsyncOperation asyncNext = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            asyncNext.allowSceneActivation = false;
            asyncNext.completed += (AsyncOperation operation) => {
                CurrScene.MainCamera = Camera.main;
                CurrScene.Init(Param);
            };

            // 다음 씬을 로딩.
            while (asyncNext.progress < 0.9f)
            {
                percent = asyncNext.progress * 0.1f;// Mathf.Clamp01(asyncNext.progress / 0.9f);
                sceneLoading.SetPercent(percent);
                yield return null;
            }

            SceneBase changeScnene = null;
            if (scenes.TryGetValue(name, out SceneBase scene) == true)
            {
                changeScnene = scene;
            }
            else 
            {
                changeScnene = CreateSceneObject(name);
                scenes.Add(name, changeScnene);

                var task = Task.Run(() => changeScnene.Load());
                bool complete = false;
                while (complete == false)
                {
                    percent = changeScnene.Amount;
                    sceneLoading.SetPercent(percent);
                    complete = percent == 1.0f && sceneLoading.Complete();
                    yield return null;
                }
            }

            sceneLoading.SetPercent(1);
            while (sceneLoading.Complete() == false)
            {
                yield return null;
            }
   
            asyncNext.allowSceneActivation = true;
            CurrScene = changeScnene;
        }
        else 
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            async.allowSceneActivation = true;
            async.completed += (AsyncOperation operation) => {
                SceneBase changeScnene = null;
                if (scenes.TryGetValue(name, out SceneBase scene) == true)
                {
                    changeScnene = scene;
                }
                else 
                {
                    changeScnene = CreateSceneObject(name);
                    scenes.Add(name, changeScnene);
                }
              
                CurrScene = changeScnene;
                CurrScene.MainCamera = Camera.main;
                CurrScene.Init(Param);
            };
        }

        Debug.Log($"{TAG} Scene Load Complete");
    }

     
    private SceneLoading GetSceneLoading(string name)
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.name.CompareTo(name) != 0)
            return null;

        var list = activeScene.GetRootGameObjects();
        foreach (var obj in list)
        {
            var scene = obj.GetComponent<SceneLoading>();
            if (scene != null)
            {
                return scene;
            }
        }
        /*
        //if (scenes.TryGetValue(name, out SceneBase.SCENES selectScene) == true)
        {
            if (activeScene.name.CompareTo(name) != 0)
                return null;

  
        }
        */

        return null;
    }

    private SceneBase CreateSceneObject(string name)
    {
        if (scenes.TryGetValue(name, out SceneBase selectScene) == false)
        {
            SceneBase scene = null; //GetSceneComponent(name);
            switch (StringToEnum<SceneBase.SCENES>(name))
            {
                case SceneBase.SCENES.SceneIntro:
                    scene = new SceneIntro();
                    break;
                case SceneBase.SCENES.SceneMenu:
                    scene = new SceneMenu();
                    break;
                case SceneBase.SCENES.SceneLoading:
                    //CurrScene = new SceneLoading();
                    break;

                case SceneBase.SCENES.SceneGostop:
                    scene = new SceneGostop();
                    break;
                case SceneBase.SCENES.SceneTileMap:
                    scene = new SceneTileMap();
                    break;
                case SceneBase.SCENES.SceneAntHouse:
                    scene = new SceneAntHouse();
                    break;
                case SceneBase.SCENES.game:
                    scene = new SceneMatch3();
                    break;
                case SceneBase.SCENES.SceneChatScroll:
                    scene = new SceneChatScroll();
                    break;
                case SceneBase.SCENES.SceneTest:
                    scene = new SceneTest();
                    break;
            }

            return scene;
        }

        return null;
    }

    public void ChangeScene(SceneBase.SCENES scene, bool loading = true, JSONObject param = null)
    {
        //var info = scenes.Where(e => e.Value == scene).First();
        //var name = info.Key;
        Param = param; // 파라미터 설정.

        string sceneName = scene.ToString();
        StartCoroutine(LoadScene(sceneName, loading));
        //Debug.Log($"{TAG} ChangeScene. {name}, {info.Value}");
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

        if (CurrScene != null)
        {
            CurrScene.OnUpdate();
        }

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


