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
        //private Dictionary<string, SceneBase> _scenes = null;
        [SerializeField] private SceneBase _currScene = null;

        float loadingPercent = 0f;

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
            //_scenes = new Dictionary<string, SceneBase>();
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

        private IEnumerator UpdateLoadPercent(UILoadingMenu loading)
        {
            bool done = false;
            while (!done)
            {
                loading?.SetPercent(loadingPercent);

                if (loadingPercent >= 1.0f) {
                    done = true;
                }

                yield return null;
            }
        }

        private IEnumerator InitScene(UILoadingMenu loading)
        {
            // 나머지 부족한 게이지를 1.0까지 채움.
            while (loading.Complete() == false)
            {
                loadingPercent += 0.1f;
                if (loadingPercent >= 1.0f)
                    loadingPercent = 1.0f;

                loading.SetPercent(loadingPercent);
                yield return null;
            }

            loading.Close();
            CurrScene.Init(Param);
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
            CurrScene?.UnLoad();
            CurrScene = null;

            UIManager.Instance.Clear();
            AsyncOperation asyncNextOperator = null;
            if (loading == true)
            {
                loadingPercent = 0f;

                // 로딩 메뉴를 띄우고 수치를 갱신.
                var loadingMenu = UIManager.Instance.OpenMenu<UILoadingMenu>("UILoadingMenu") as UILoadingMenu;
                var handle = StartCoroutine(UpdateLoadPercent(loadingMenu));

                // 다음 씬을 로드 시작.
                asyncNextOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = false;
                asyncNextOperator.completed += (AsyncOperation operation) => {
                    CurrScene = CreateSceneObject(sceneName);
                    CurrScene.MainCamera = Camera.main;
                    
                    // 비동기로 데이터 로드를 하고, 완료되면 초기화.
                    Task.Run(() => {
                        CurrScene.Load((percent) => {
                            loadingPercent = percent;
                        });
                    }).
                    // 테스크 완료후 동기로 받음.
                    ContinueWith(preTask => {
                        StopCoroutine(handle);
                        StartCoroutine(InitScene(loadingMenu));
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                };
                
                // 다음 씬 로드 게이지를 갱신.
                while (asyncNextOperator.progress < 0.9f)
                {
                    loadingPercent = asyncNextOperator.progress;
                    yield return null;
                }

                // Activation on.
                asyncNextOperator.allowSceneActivation = true;
            }
            else
            {
                asyncNextOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = true;
                asyncNextOperator.completed += (AsyncOperation operation) => {
                    SceneBase changeScnene = CreateSceneObject(sceneName);
                    CurrScene = changeScnene;
                    CurrScene.MainCamera = Camera.main;

                    Task.Run(() => {
                        CurrScene.Load((percent) => {
                            loadingPercent = percent;
                        });
                    }).
                    ContinueWith(preTask => {
                        CurrScene.Init(Param);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                };

                yield return null;
            }
            
            //Debug.Log($"{Tag} Scene Load Complete");
        }


        private SceneLoading GetSceneLoading(string sceneName)
        {
            var activeScene = SceneManager.GetActiveScene();
            if (String.Compare(activeScene.name, sceneName, StringComparison.Ordinal) != 0)
                return null;

            var list = activeScene.GetRootGameObjects();
            return list.Select(obj => obj.GetComponent<SceneLoading>()).FirstOrDefault(scene => scene != null);
        }

        /// <summary>
        /// 씬객체를 붙이거나, 있으면 참조 시켜줍니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>참조된 씬 객체</returns>
        private SceneBase CreateSceneObject(string sceneName)
        {
            SceneBase scene = null; //GetSceneComponent(name);
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var root = activeScene.GetRootGameObjects()[0];

            GameObject sceneObject = GameObject.Find(sceneName);
            if (sceneObject == null)
            {
                var obj = new GameObject(sceneName);
                obj.transform.SetParent(root.transform.parent);

                switch (StringToEnum<SceneBase.Scenes>(sceneName))
                {
                    case SceneBase.Scenes.SceneIntro:
                        scene = obj.AddComponent<SceneIntro>(); //new SceneIntro();
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
                    case SceneBase.Scenes.Demo:
                        scene = obj.AddComponent<SceneDemo>();
                        break;
                    case SceneBase.Scenes.SceneMaze:
                        scene = obj.AddComponent<SceneMaze>();
                        break;
                    case SceneBase.Scenes.SceneBehaviorTree:
                        scene = obj.AddComponent<SceneBehaviorTree>();
                        break;

                    default:
                        break;
                }
            }
            else 
            {
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


