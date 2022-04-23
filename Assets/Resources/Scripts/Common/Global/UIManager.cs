using Singleton;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private CanvasGroup canvasMain;
    private CanvasController controllerMenu = null;
    private CanvasController controllerHud = null;
    private CanvasController controllerPopup = null;
    private CanvasController controllerEtc = null;

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

        canvasMain = root.GetComponent<CanvasGroup>();

        controllerMenu = new CanvasController();
        controllerMenu.Init(root.transform.Find("Canvas - ui"));
        controllerHud = new CanvasController();
        controllerHud.Init(root.transform.Find("Canvas - hud"));
        controllerPopup = new CanvasController();
        controllerPopup.Init(root.transform.Find("Canvas - popup"));
        controllerEtc = new CanvasController();
        controllerEtc.Init(root.transform.Find("Canvas - etc"));

        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    public T OpenMenu<T>(string name)
    {
        return controllerMenu.Open<T>(name);
    }

    public T OpenPopup<T>(string name)
    {
        return controllerPopup.Open<T>(name);
    }

    public T OpenHud<T>(string name)
    {
        return controllerHud.Open<T>(name);
    }

    public T OpenEtc<T>(string name)
    {
        return controllerEtc.Open<T>(name);
    }

    public void CloseMenu(string name)
    {
        controllerMenu.Close(name);
    }

    public void ClosePopup(string name)
    {
        controllerPopup.Close(name);
    }

    public void CloseHud(string name)
    {
        controllerHud.Close(name);
    }

    public void CloseEtc(string name)
    {
        controllerEtc.Close(name);
    }

    public void Clear()
    {
        controllerMenu.Clear();
        controllerHud.Clear();
        controllerPopup.Clear();
        controllerEtc.Clear();
    }

    public void BackKey()
    {
        if(controllerEtc.Last() != null)
        {
            controllerEtc.Close(controllerEtc.Last());
        }
        else if (controllerPopup.Last() != null)
        {
            controllerPopup.Close(controllerPopup.Last());
        }
        else
        {
            // 더이상 닫을 팝업이 없음.
        }
    }
}

public class CanvasController
{
    Canvas canvas = null;
    CanvasGroup group = null;

    public void Init(Transform trans)
    {
        canvas = trans.GetComponent<Canvas>();
        group = trans.GetComponent<CanvasGroup>();
    }

    public T Open<T>(string name)
    {
        string path = string.Format("Prefabs/UI/{0}", name);
        GameObject prefab = ResourcesManager.Instance.Load(path);
        if (prefab == null)
        {
            Debug.Log($"{path} prefab is null.");
            return default(T);
        }

        GameObject clone = GameObject.Instantiate(prefab, canvas.transform);
        if (clone == null)
        {
            Debug.Log($"ui is null.");
            return default(T);
        }

        clone.name = name;

        UIObject obj = clone.GetComponent<UIObject>();
        obj.OnOpen();

        return clone.GetComponent<T>();
    }

    public void Close(string name)
    {
        var trans = canvas.transform.Find(name);
        if (trans != null)
        {
            UIObject obj = trans.GetComponent<UIObject>();
            obj.OnClose();

            GameObject.Destroy(trans.gameObject);
        }
    }

    public void Close(Transform transform)
    {
        Close(transform.name);
    }

    public Transform Get(string name)
    {
        return canvas.transform.Find(name);
    }

    public Transform Last()
    {
        int index = canvas.transform.childCount - 1;
        Transform trans = null;
        if(index >= 0)
        {
            trans = canvas.transform.GetChild(index);
        }
        
        return trans;
    }

    public void Clear()
    {
        for (int i = 0; i < canvas.transform.childCount ;i++)
        {
            Transform trans = canvas.transform.GetChild(0);
            GameObject.Destroy(trans.gameObject);
        }
    }
}
