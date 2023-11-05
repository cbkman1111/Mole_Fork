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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator LoadScene(string sceneName)
        {
            CurrScene = null;
            yield return null;

            //UIManager.Instance.Clear();

            var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = true;
            async.completed += (AsyncOperation operation) => {

                var root = GetRoot();
                var changeScnene = GetScene(sceneName);
                UIManager.Instance.SetRoot(root);

                CurrScene = changeScnene;
                CurrScene.MainCamera = Camera.main;
                CurrScene.Init(Param);

                if (Param != null)
                {
                    ChangeScene(SceneBase.Scenes.SceneLobby);
                    ChangeScene(SceneBase.Scenes.SceneInGame);
                    Param = null;
                }
            };

            yield return null;
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

        private SceneBase GetScene(string name)
        {
            if (_scenes.TryGetValue(name, out var selectScene) != false)
            {
                return selectScene;
            }

            var scene = SceneManager.GetActiveScene();
            var objects = scene.GetRootGameObjects();
            SceneBase sceneObject = null;
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "Scene")
                {
                    sceneObject = objects[i].GetComponent<SceneBase>();
                    break;
                }
            }

            return sceneObject;
        }

        private GameObject GetRoot()
        {
            GameObject root = null;
            var scene = SceneManager.GetActiveScene();
            var objects = scene.GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "UIRoot")
                {
                    root = objects[i];
                    break;
                }
            }

            return root;
        }

        private SceneBase CreateSceneObject(string sceneName)
        {
            if (_scenes.TryGetValue(sceneName, out var selectScene) != false)
            {
                return null;
            }
            
            SceneBase scene = null;
            switch (StringToEnum<SceneBase.Scenes>(sceneName))
            {
                case SceneBase.Scenes.SceneIntro:
                    scene = new SceneIntro();
                    break;
            }

            return scene;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loading"></param>
        /// <param name="param"></param>
        public void ChangeScene(SceneBase.Scenes scene, bool loading = true, JSONObject param = null)
        {
            Param = param; 

            var sceneName = scene.ToString();
            StartCoroutine(LoadScene(sceneName));
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


