using UnityEngine;

namespace Common.Scene
{
    public abstract class SceneBase //: MonoBehaviour
    {
        public enum Scenes
        {
            SceneIntro = 0,
            SceneMenu,
            SceneLoading,
            SceneGostop,
            SceneTileMap,
            SceneAntHouse,
            game,
            SceneChatScroll,
            SceneTest,
            
            SceneBundle, // 번들 테스트 씬. 어드레스블번들, 에셋번들.
            SceneMaze, // Behavior Design 테스트용 미로 찾기.

            Demo
        }

        public float Amount { get; set; } = 0;

        public Camera MainCamera { get; set; }

        public virtual void UnLoad() { }

        public virtual void LoadBeforeAsync()
        {
        }

        public virtual async void Load()
        {
            Amount = 1f;
        }

        public virtual void OnUpdate() { }
        public abstract bool Init(JSONObject param);
    
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchBean(Vector3 position) { }
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchMove(Vector3 position) { }
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchEnd(Vector3 position) { }
    }
}
