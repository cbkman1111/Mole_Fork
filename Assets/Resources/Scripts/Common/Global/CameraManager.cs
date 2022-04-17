using Singleton;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{    
    public Camera MainCamera { get; set; }

    public override bool Init()
    {
        DontDestroyOnLoad(gameObject);

        gameObject.name = string.Format("singleton - {0}", TAG);
        MainCamera = gameObject.AddComponent<Camera>() as Camera;

        MainCamera.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        MainCamera.transform.position = new Vector3(0, 5f, -1);
        MainCamera.orthographic = true;

        MainCamera.farClipPlane = 40f;
        MainCamera.nearClipPlane = 0.1f;
        return true;
    }
}
