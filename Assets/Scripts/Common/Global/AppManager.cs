using Common.Global.Singleton;
using Common.Scene;
using Common.Utils;
using Common.Utils.Pool;
using Network;
using Scenes;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Global
{
    public class AppManager : MonoSingleton<AppManager>
    {
        private float _loadingPercent = 0f;
        private SceneBase _currScene = null;
        public SceneBase CurrScene
        {
            get => _currScene;
        }

        private JSONObject _param { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Init()
        {
            SoundManager.Instance.Load();
            DataManager.Instance.Load();
            ResourcesManager.Instance.Load();
            //NetworkManager.Instance.Connect();
            return true;
        }

        public void StartApplication()
        {
            //지금 활성화 된 씬 가져오기.
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var startScene = StringToEnum<SceneBase.Scenes>(activeScene.name);
            
            AppManager.Instance.ChangeScene(startScene, false);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator UpdateLoadPercent(UILoadingMenu loading)
        {
            bool done = false;
            while (!done)
            {
                loading.SetPercent(_loadingPercent);

                if (_loadingPercent >= 1.0f) {
                    done = true;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator InitScene(UILoadingMenu loading)
        {
            yield return new WaitForSeconds(0.2f);

            // 나머지 부족한 게이지를 1.0까지 채움.
            while (loading.Complete() == false)
            {
                _loadingPercent += 0.1f;
                if (_loadingPercent >= 1.0f)
                    _loadingPercent = 1.0f;

                loading.SetPercent(_loadingPercent);
                yield return null;
            }

            loading.Close();
            CurrScene.Init(_param);
            yield return null;
        }

        /// <summary>
        /// 로딩중 팝업을 출력하고 대상 씬을 로드합니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator LoadScene(string sceneName, bool loading)
        {
            GiantDebug.Log("AppManager - LoadScene 1");
            _loadingPercent = 0f;
            if (_currScene != null)
            {
                _currScene.UnLoad();
                _currScene = null;
            }

            UIManager.Instance.Clear();
            AsyncOperation asyncNextOperator = null;
            if (loading == true)
            {
                GiantDebug.Log("AppManager - LoadScene 2");

                // 로딩 메뉴를 띄우고 수치를 갱신.
                var loadingMenu = UIManager.Instance.OpenMenu<UILoadingMenu>();
                var handle = StartCoroutine(UpdateLoadPercent(loadingMenu));

                // 다음 씬을 로드 시작.
                asyncNextOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = true;
                asyncNextOperator.completed += (AsyncOperation operation) => {
                    
                    GiantDebug.Log("AppManager - LoadScene 2 - 1");

                    _currScene = CreateSceneObject(sceneName);
                    _currScene.MainCamera = Camera.main;
                    
                    // 비동기로 데이터 로드를 하고, 완료되면 초기화.
                    Task.Run(() => {
                        CurrScene.Load((percent) => {
                            _loadingPercent = percent;
                        });
                    }).
                    // 테스크 완료후 동기로 받음.
                    ContinueWith(preTask => {

                        GiantDebug.Log("AppManager - LoadScene 2 - 3");
                        StopCoroutine(handle);
                        StartCoroutine(InitScene(loadingMenu));
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                };
            }
            else
            {
                GiantDebug.Log("AppManager - LoadScene 3");

                asyncNextOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = true;
                asyncNextOperator.completed += (AsyncOperation operation) => {
                    Debug.Log("AppManager - LoadScene 3 - 1");

                    SceneBase changeScnene = CreateSceneObject(sceneName);
                    if (changeScnene == null)
                        Debug.LogError("AppManager - 3 - 2 changeScnene  is null.");

                    _currScene = changeScnene;
                    _currScene.MainCamera = Camera.main;
                    Task.Run(() => {
                        Debug.Log("AppManager - LoadScene 3 - 2");
                        _currScene.Load((percent) => {
                            _loadingPercent = percent;

                            Debug.Log("AppManager - LoadScene 3 - 3");
                        });
                    }).
                    ContinueWith(preTask => {
                        Debug.Log("AppManager - LoadScene 3 - 4");

                       
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                     _currScene.Init(_param);
                };

                yield return null;
            }
        }

        /// <summary>
        /// 씬객체를 붙이거나, 있으면 참조 시켜줍니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>참조된 씬 객체</returns>
        private SceneBase CreateSceneObject(string sceneName)
        {
            Debug.Log("AppManager - CreateSceneObject 1");

            SceneBase scene = null; //GetSceneComponent(name);
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var root = activeScene.GetRootGameObjects()[0];

            GameObject sceneObject = GameObject.Find(sceneName);
            if (sceneObject == null)
            {
                Debug.Log("AppManager - CreateSceneObject 1 - 1");

                var obj = new GameObject(sceneName);
                obj.transform.SetParent(root.transform.parent);

                switch (StringToEnum<SceneBase.Scenes>(sceneName))
                {
                    case SceneBase.Scenes.SceneIntro:
                        scene = obj.AddComponent<SceneIntro>();
                        break;
                    case SceneBase.Scenes.SceneMenu:
                        scene = obj.AddComponent<SceneMenu>();
                        break;
                    case SceneBase.Scenes.SceneGostop:
                        scene = obj.AddComponent<SceneGostop>();
                        break;
                    case SceneBase.Scenes.SceneTileMap:
                        scene = obj.AddComponent<SceneTileMap>();
                        break;
                    case SceneBase.Scenes.SceneAntHouse:
                        scene = obj.AddComponent<SceneAntHouse>();
                        break;
                    case SceneBase.Scenes.game:
                        scene = obj.AddComponent<SceneMatch3>();
                        break;
                    case SceneBase.Scenes.SceneChatScroll:
                        scene = obj.AddComponent<SceneChatScroll>();
                        break;
                    case SceneBase.Scenes.SceneTest:
                        scene = obj.AddComponent<SceneTest>();
                        break;
                    case SceneBase.Scenes.SceneBundle:
                        scene = obj.AddComponent<SceneBundle>();
                        break;
                    case SceneBase.Scenes.SceneMaze:
                        scene = obj.AddComponent<SceneMaze>();
                        break;
                    case SceneBase.Scenes.SceneBehaviorTree:
                        scene = obj.AddComponent<SceneBehaviorTree>();
                        break;
                    case SceneBase.Scenes.ScenePuzzle:
                        scene = obj.AddComponent<ScenePuzzle>();
                        break;
                    case SceneBase.Scenes.SceneTetris:
                        scene = obj.AddComponent<SceneTetris>();
                        break;
                    case SceneBase.Scenes.SceneDotween:
                        scene = obj.AddComponent<SceneDotween>();
                        break;
                    case SceneBase.Scenes.SceneMatch3:
                        scene = obj.AddComponent<SceneMatch3>();
                        break;
                    case SceneBase.Scenes.SceneHash:
                        scene = obj.AddComponent<SceneHash>();
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                Debug.Log("AppManager - CreateSceneObject 1 - 2");
                scene = sceneObject.GetComponent<SceneBase>();
            }
            
            return scene;
        }

        /// <summary>
        /// 씬 전환.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loading"></param>
        /// <param name="param"></param>
        public void ChangeScene(SceneBase.Scenes scene, bool loading = true, JSONObject param = null)
        {
            _param = param; 
            var sceneName = scene.ToString();
            StartCoroutine(LoadScene(sceneName, loading));
        }


        void OnApplicationPause(bool paused)
        {
            
        }

        void OnApplicationFocus(bool focus)
        {

        }

        private void OnApplicationQuit()
        {
            SoundManager.Instance.Destroy();
            DataManager.Instance.Destroy();
            NetworkManager.Instance.Destroy();
            ResourcesManager.Instance.Destroy();
            UIManager.Instance.Destroy();
            PoolManager.Instance.Destroy();

            AppManager.Instance.Destroy();
            Debug.Log("App Quit");
        }


        private Vector3 lastMousePosition = Vector3.zero;

        private void Update()
        {
            var touches = Input.touches;
            var phase = TouchPhase.Began;

            CurrScene?.OnUpdate();

            if (touches.Count() == 1)
            {
                var touch = touches[0]; 
                phase = touch.phase;

                Vector3 position = touch.position;
                if (phase == TouchPhase.Began)
                {
                    OnTouchBean(position);
                    lastMousePosition = position;
                }
                else if (phase == TouchPhase.Moved ||
                        phase == TouchPhase.Stationary)
                {
                    OnTouchMove(position, lastMousePosition - position);
                    lastMousePosition = position;
                }
                /*
                else if (phase == TouchPhase.Stationary)
                {
                    OnTouchStationary(position);
                }
                */
                else if (phase == TouchPhase.Ended)
                {
                    OnTouchEnd(position);
                    lastMousePosition = Vector3.zero;
                }
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
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                OnTouchMove(Input.mousePosition, lastMousePosition - Input.mousePosition);
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnd(Input.mousePosition);
                lastMousePosition = Vector3.zero;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackKeyDown();
            }
        }

        private void OnTouchBean(Vector3 position)
        {
            CurrScene?.OnTouchBean(position);
        }

        private void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {
            CurrScene?.OnTouchMove(position, deltaPosition);
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

        private void OnBackKeyDown()
        {
            UIManager.Instance.BackKey();
        }


    }
}


