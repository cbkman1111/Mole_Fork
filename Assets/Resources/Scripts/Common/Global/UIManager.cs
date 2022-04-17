using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private CanvasGroup canvasMain;
    private CanvasGroup canvasUI;
    private CanvasGroup canvasHud;
    private CanvasGroup canvasPopup;
    private CanvasGroup canvasEtc;

    private GameObject root = null;

    public override bool Init()
    {
        const string PATH_UI_ROOT = "Prefabs/UI/UIRoot";
        GameObject obj = ResourcesManager.Instance.Load(PATH_UI_ROOT);
        root = Instantiate(obj, transform);
        if(root == null)
        {
            Debug.LogError($"root is null.");
            return false;
        }

        root.name = "UIRoot";
        root.transform.position = new Vector3(100,0,0);
        gameObject.name = string.Format("singleton - {0}", TAG);

        canvasMain = root.GetComponent<CanvasGroup>();
        canvasUI = root.transform.Find("Canvas - ui").gameObject.GetComponent<CanvasGroup>();
        canvasHud = root.transform.Find("Canvas - hud").gameObject.GetComponent<CanvasGroup>();
        canvasPopup = root.transform.Find("Canvas - popup").gameObject.GetComponent<CanvasGroup>();
        canvasEtc = root.transform.Find("Canvas - etc").gameObject.GetComponent<CanvasGroup>();
        return true;
    }

    /// <summary>
    /// Canvas - ui
    /// </summary>
    /// <param name="name">Prefabs/UI/리소스명</param>
    /// <returns>ui GameObject</returns>
    public T OpenUI<T>(string name)
    {
        string path = string.Format("Prefabs/UI/{0}", name);
        GameObject prefab = ResourcesManager.Instance.Load(path);
        if(prefab == null)
        {
            Debug.Log($"{path} prefab is null.");
            return default(T);
        }

        GameObject ui = Instantiate(prefab, canvasUI.transform);
        if(ui == null)
        {
            Debug.Log($"ui is null.");
            return default(T);
        }

        UIBase obj = ui.GetComponent<UIBase>();
        obj.name = name;
        return obj.GetComponent<T>();
    }

    public void CloseUI(string name)
    {
        var trans = canvasUI.transform.Find(name);
        if(trans != null)
        {
            GameObject.Destroy(trans.gameObject);
        }
    }

    public void ClosePopup(string name)
    {
        var trans = canvasPopup.transform.Find(name);
        if (trans != null)
        {
            GameObject.Destroy(trans.gameObject);
        }
    }

    public T OpenPopup<T>(string name)
    {
        string path = string.Format("Prefabs/UI/{0}", name);
        GameObject prefab = ResourcesManager.Instance.Load(path);
        if (prefab == null)
        {
            Debug.Log($"{path} prefab is null.");
            return default(T);
        }

        GameObject ui = Instantiate(prefab, canvasPopup.transform);
        if (ui == null)
        {
            Debug.Log($"ui is null.");
            return default(T);
        }

        PopupBase obj = ui.GetComponent<PopupBase>();
        obj.name = name;
        return obj.GetComponent<T>();
    }

    public void RemoveAll()
    {
        foreach (var ui in canvasUI.GetComponentsInChildren<UIBase>())
        {
            GameObject.Destroy(ui.gameObject);
        }
    }

    public GameObject GetLastPopup()
    {
        return null;
    }

    public GameObject GetPopup(string name)
    {
        return canvasPopup.transform.Find(name).gameObject;
    }
}
