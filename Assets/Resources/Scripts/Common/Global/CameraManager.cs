using Singleton;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{    
    public Camera MainCamera { get; set; }

    public override bool Init()
    {
        MainCamera = gameObject.AddComponent<Camera>() as Camera;
        MainCamera.transform.position = new Vector3(0, 0, -1);
        MainCamera.orthographic = true;
        MainCamera.farClipPlane = 10f;
        MainCamera.nearClipPlane = 0.1f;

        DontDestroyOnLoad(MainCamera);
        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }
}
