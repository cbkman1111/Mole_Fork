using Common.Global.Singleton;
using Common.Scene;
using Common.Utils.Pool;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Global
{
    public class AppManager : MonoSingleton<AppManager>
    {
        private float _loadingPercent = 0f;
        private SceneBase _currScene = null;

        /// <summary>
        /// 국가별 로컬라이즈 설정.
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = new CultureInfo("ko-KR");


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
            //SoundManager.Instance.Load();
            //DataManager.Instance.Load();
            //ResourcesManager.Instance.Load();
            //_ = AdMobManager.Instance;
            //NetworkManager.Instance.Connect();

            return true;
        }

        public void StartApplication()
        {
            SceneBase.Scenes startScene = SceneBase.Scenes.SceneIntro;

#if UNITY_EDITOR
            startScene = StringToEnum<SceneBase.Scenes>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
#endif
            ChangeScene(startScene, false);
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
        //private IEnumerator UpdateLoadPercent(UILoadingMenu loading)
        private IEnumerator<float> UpdatePercent(UIPopupLoading loading)
		{
			yield return MEC.Timing.WaitForOneFrame;
            bool done = false;
            while (!done)
            {
                loading.SetPercent(_loadingPercent);
                if (_loadingPercent >= 1.0f) {
                    done = true;
                }

                yield return MEC.Timing.WaitForOneFrame;
            }
        }


        /// <summary>
        /// 로딩중 팝업을 출력하고 대상 씬을 로드합니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator LoadScene(string sceneName, bool loading)
        {
            _loadingPercent = 0f;
            if (_currScene != null)
            {
                _currScene.UnLoad();
                _currScene = null;
            }
            
            AsyncOperation asyncNextOperator = null;
            if (loading == true)
            {
                // 로딩 메뉴를 띄우고 수치를 갱신.
                var loadingMenu = UIManager.Instance.OpenDontDesroyPopup<UIPopupLoading>();
                var gameObjectLoading = loadingMenu.gameObject;
                var handlerLoading = MEC.Timing.RunCoroutine(UpdatePercent(loadingMenu).CancelWith(loadingMenu));

                // 비동기 씬 로딩 시작.
                asyncNextOperator = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = false;

                bool loadDone = false;
                while (loadDone == false)
                {
                    _loadingPercent = asyncNextOperator.progress;

                    if (asyncNextOperator.progress >= 0.9f)
                        loadDone = true;
                    
                    yield return null;
                }

                asyncNextOperator.allowSceneActivation = true;
                yield return new WaitUntil(() => asyncNextOperator.isDone == true);
                
                
                _currScene = FindSceneObject(sceneName);
                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                UIManager.Instance.InitWithScene(activeScene);

                _currScene.Load((percent) => {
                    _loadingPercent = 0.9f + (0.1f * percent);
                });

                yield return new WaitUntil(() => loadingMenu.Complete(1.0f) == true);
                yield return new WaitForEndOfFrame();

                UIManager.Instance.CloseDontDestroyPopup<UIPopupLoading>();
                _currScene.MainCamera = Camera.main;
                _currScene.Init(_param);

                yield return new WaitForEndOfFrame();
                Resources.UnloadUnusedAssets();
            }
            else
            {
                asyncNextOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncNextOperator.allowSceneActivation = false;

                bool loadDone = false;
                while (loadDone == false)
                {
                    _loadingPercent = asyncNextOperator.progress;

                    if (asyncNextOperator.progress >= 0.9f)
                        loadDone = true;

                    yield return null;
                }

                _currScene = FindSceneObject(sceneName);
                UIManager.Instance.InitWithScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                _currScene.Load((percent) => {
                    _loadingPercent = 0.9f + (0.1f * percent);
                });

                yield return new WaitUntil(() => _loadingPercent == 1.0f);
                yield return new WaitForEndOfFrame();

                _currScene.MainCamera = Camera.main;
                _currScene.Init(_param);

                yield return null;
            }
        }

        /// <summary>
        /// 씬객체를 붙이거나, 있으면 참조 시켜줍니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>참조된 씬 객체</returns>
        private SceneBase FindSceneObject(string sceneName)
        {
            SceneBase scene = null;
            var activeScene = SceneManager.GetActiveScene();
            var count = SceneManager.sceneCount;
            for(int i = 0; i < count; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.name == sceneName)
                {
                    activeScene = s;
                    break;
                }
            }

            var objects = activeScene.GetRootGameObjects();
            for(int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                if (obj.name == sceneName)
                {
                    scene = obj.GetComponent<SceneBase>();
                    if (scene != null)
                        break;
                }
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
            AdMobManager.Instance.Destroy();
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


