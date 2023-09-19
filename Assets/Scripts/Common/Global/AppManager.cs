using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Global.Singleton;
using Common.Scene;
using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Global
{
    public class AppManager : MonoSingleton<AppManager>
    {
        private Dictionary<string, SceneBase> _scenes = null;
        [SerializeField] private SceneBase _currScene = null;

        public SceneBase CurrScene
        {
            get => _currScene;
            set => _currScene = value;
        }

        private JSONObject Param { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Init()
        {
            _scenes = new Dictionary<string, SceneBase>();
            gameObject.name = $"singleton - {Tag}";
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        private T StringToEnum<T>(string e)
        {
            return (T)Enum.Parse(typeof(T), e);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator LoadScene(string sceneName, bool loading)
        {
            CurrScene?.UnLoad();

            UIManager.Instance.Clear();
            CurrScene = null;
            if (loading == true)
            {
                var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SceneLoading", LoadSceneMode.Single);
                async.allowSceneActivation = true;
                float percent = 0;

                //SceneLoading sceneLoading = new SceneLoading();
                while (async.isDone == false || async.progress < 1.0f)
                {
                    yield return null;
                }

                var sceneLoading = GetSceneLoading("SceneLoading");
                var asyncNext = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNext.allowSceneActivation = false;
                asyncNext.completed += (AsyncOperation operation) => {
                    if (CurrScene != null)
                    {                    
                        CurrScene.MainCamera = Camera.main;
                        CurrScene.Init(Param);
                    }
                };
    
                // ���� ���� �ε�.
                while (asyncNext.progress < 0.9f)
                {
                    percent = asyncNext.progress * 0.1f;// Mathf.Clamp01(asyncNext.progress / 0.9f);
                    sceneLoading.SetPercent(percent);
                    yield return null;
                }
            
                SceneBase changeScene = null;
                if (_scenes.TryGetValue(sceneName, out var scene) == true)
                {
                    changeScene = scene;
                }
                else 
                {
                    changeScene = CreateSceneObject(sceneName);
                    _scenes.Add(sceneName, changeScene);
                
                    var task = Task.Run(() => changeScene.Load());
                    bool complete = false;
                    while (complete == false)
                    {
                        percent = changeScene.Amount;
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
                CurrScene = changeScene;
            }
            else 
            {
                var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                async.allowSceneActivation = true;
                async.completed += (AsyncOperation operation) => {
                    SceneBase changeScnene = null;
                    if (_scenes.TryGetValue(sceneName, out SceneBase scene) == true)
                    {
                        changeScnene = scene;
                    }
                    else 
                    {
                        changeScnene = CreateSceneObject(sceneName);
                        _scenes.Add(sceneName, changeScnene);
                    }
              
                    CurrScene = changeScnene;
                    CurrScene.MainCamera = Camera.main;
                    CurrScene.Init(Param);
                };
            }

            Debug.Log($"{Tag} Scene Load Complete");
        }

     
        private SceneLoading GetSceneLoading(string sceneName)
        {
            var activeScene = SceneManager.GetActiveScene();
            if (String.Compare(activeScene.name, sceneName, StringComparison.Ordinal) != 0)
                return null;

            var list = activeScene.GetRootGameObjects();
            return list.Select(obj => obj.GetComponent<SceneLoading>()).FirstOrDefault(scene => scene != null);
        }

        private SceneBase CreateSceneObject(string sceneName)
        {
            if (_scenes.TryGetValue(sceneName, out var selectScene) != false) return null;
            
            SceneBase scene = null; //GetSceneComponent(name);
            switch (StringToEnum<SceneBase.Scenes>(sceneName))
            {
                case SceneBase.Scenes.SceneIntro:
                    scene = new SceneIntro();
                    break;
                case SceneBase.Scenes.SceneMenu:
                    scene = new SceneMenu();
                    break;
                case SceneBase.Scenes.SceneLoading:
                    //CurrScene = new SceneLoading();
                    break;

                case SceneBase.Scenes.SceneGostop:
                    scene = new SceneGostop();
                    break;
                case SceneBase.Scenes.SceneTileMap:
                    scene = new SceneTileMap();
                    break;
                case SceneBase.Scenes.SceneAntHouse:
                    scene = new SceneAntHouse();
                    break;
                case SceneBase.Scenes.game:
                    scene = new SceneMatch3();
                    break;
                case SceneBase.Scenes.SceneChatScroll:
                    scene = new SceneChatScroll();
                    break;
                case SceneBase.Scenes.SceneTest:
                    scene = new SceneTest();
                    break;
            }

            return scene;

        }

        public void ChangeScene(SceneBase.Scenes scene, bool loading = true, JSONObject param = null)
        {
            Param = param; 

            var sceneName = scene.ToString();
            StartCoroutine(LoadScene(sceneName, loading));
        }

        public SceneBase GetCurrentScene()
        {
            return CurrScene;
        }

        public void OnAppPause(bool paused)
        {
            Debug.Log($"{Tag} OnAppPause. {paused}");
        }

        public void OnAppFocus(bool focus)
        {
            Debug.Log($"{Tag} OnAppFocus. {focus}");
        }

        public void OnAppQuit()
        {
            Debug.Log($"{Tag} OnAppQuit.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void Update()
        {
            var touches = Input.touches;
            var phase = TouchPhase.Began;

            CurrScene?.OnUpdate();

            if (touches.Count() == 1)
            {
                var touch = touches[0]; 
                phase = touch.phase;
                var position = touch.position;
                if (phase == TouchPhase.Began)
                    OnTouchBean(position);
                else if (phase == TouchPhase.Moved)
                    OnTouchMove(position);
                else if (phase == TouchPhase.Stationary)
                    OnTouchStationary(position);
                else if (phase == TouchPhase.Ended)
                    OnTouchEnd(position);
                else if (phase == TouchPhase.Canceled)
                    OnTouchCancle(position);
            }
            else if (touches.Count() == 2)
            {
                //Debug.Log($"{TAG} TOUCH 0 - {touches[0].fingerId}");
                //Debug.Log($"{TAG} TOUCH 1 - {touches[1].fingerId}");
            }
            else if (Input.GetMouseButtonDown(0))
            {
                OnTouchBean(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                OnTouchMove(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnd(Input.mousePosition);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackKeyDown();
            }
        }

        private void OnTouchBean(Vector3 position)
        {
            CurrScene?.OnTouchBean(position);
        }

        private void OnTouchMove(Vector3 position)
        {
            CurrScene?.OnTouchMove(position);
        }

        private void OnTouchEnd(Vector3 position)
        {
            CurrScene?.OnTouchEnd(position);
        }

        private void OnTouchStationary(Vector3 position)
        {
        }
        
        private void OnTouchCancle(Vector3 position)
        {
        }

        private void BackKeyDown()
        {
            UIManager.Instance.BackKey();
        }
    }
}


