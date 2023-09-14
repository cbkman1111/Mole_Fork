using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase //: MonoBehaviour
{
    public enum SCENES
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
    }

    public float Amount { get; set; } = 0;

    public Camera MainCamera { get; set; }

    public virtual void UnLoad() { }

    public async virtual void Load()
    {
        Amount = 1f;
    }

    public virtual void OnUpdate() { }
    public abstract bool Init(JSONObject param);
    public virtual void OnTouchBean(Vector3 position) { }
    public virtual void OnTouchMove(Vector3 position) { }
    public virtual void OnTouchEnd(Vector3 position) { }
}
