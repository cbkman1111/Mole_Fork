using System;
using UnityEngine;

namespace Common.Scene
{
    public abstract class SceneBase : MonoBehaviour
    {
        public Camera MainCamera { get; set; }

        public virtual void UnLoad() { }

        public virtual async void Load(Action<float> update)
        {
            update(1f);
        }
  
        public virtual void OnUpdate() { }
        public abstract bool Init(JSONObject param);
    
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchBean(Vector3 position) { }
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchMove(Vector3 position, Vector2 deltaPosition) { }
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void OnTouchEnd(Vector3 position) { }
    }
}
