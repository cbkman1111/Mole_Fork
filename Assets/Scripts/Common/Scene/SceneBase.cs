using UnityEngine;

namespace Common.Scene
{
    public abstract class SceneBase : MonoBehaviour
    {
        public enum Scenes
        {
            None = -1,

            SceneIntro = 0, // 

            SceneLoading, // 게임 로딩.

            SceneLobby, // 로비.
            SceneInGame // 인게임.
        }

        public Camera MainCamera { get; set; }
        public virtual void LoadBeforeAsync()
        {
        }

        public virtual void OnUpdate() { }
        public abstract bool Init(JSONObject param);
    
        public virtual void OnTouchBean(Vector3 position) { }
        public virtual void OnTouchMove(Vector3 position) { }
        public virtual void OnTouchEnd(Vector3 position) { }
        public virtual void OnTouchStationary(Vector3 position) { }
    }
}
