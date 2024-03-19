using Common.Global;
using Common.Scene;
using UnityEngine;

public class AppDelegate : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void main()
    {
        Debug.Log("App Started. by AppDelegate.cs");
        Application.targetFrameRate = 60;

        Debug.Log("Device Model: " + SystemInfo.deviceModel);
        Debug.Log("Device Name: " + SystemInfo.deviceName);
        Debug.Log("Device Type: " + SystemInfo.deviceType);
        Debug.Log("Operating System: " + SystemInfo.operatingSystem);
        Debug.Log("Processor Count: " + SystemInfo.processorCount);
        Debug.Log("Processor Frequency: " + SystemInfo.processorFrequency);
        Debug.Log("Processor Type: " + SystemInfo.processorType);
        Debug.Log("Supported Render Texture Count: " + SystemInfo.supportedRenderTargetCount);
        Debug.Log("System Memory Size: " + SystemInfo.systemMemorySize);
        Debug.Log("Graphics Device Name: " + SystemInfo.graphicsDeviceName);
        Debug.Log("Graphics Device Vendor: " + SystemInfo.graphicsDeviceVendor);
        Debug.Log("Graphics Memory Size: " + SystemInfo.graphicsMemorySize);
        Debug.Log("Graphics Shader Level: " + SystemInfo.graphicsShaderLevel);
        Debug.Log("Supports Shadows: " + SystemInfo.supportsShadows);


        AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneIntro, false);
    }

    void OnApplicationPause(bool paused)
    {
        AppManager.Instance.OnAppPause(paused);
    }

    void OnApplicationFocus(bool focus)
    {
        AppManager.Instance.OnAppFocus(focus);
    }

    private void OnApplicationQuit()
    {
        AppManager.Instance.OnAppQuit();
    }
}
