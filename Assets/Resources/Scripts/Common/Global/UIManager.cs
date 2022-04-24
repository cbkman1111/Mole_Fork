using Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    private CanvasGroup canvasMain;
    private CanvasController controllerMenu = null;
    private CanvasController controllerHud = null;
    private CanvasController controllerPopup = null;
    private CanvasController controllerEtc = null;

    private Transform cover = null;

    public override bool Init()
    {
        const string PATH_UI_ROOT = "Prefabs/UI/UIRoot";
        GameObject prefab = ResourcesManager.Instance.Load<GameObject>(PATH_UI_ROOT);
        
        GameObject root = Instantiate(prefab, transform);
        if(root == null)
        {
            Debug.LogError($"root is null.");
            return false;
        }

        root.name = "UIRoot";
        root.transform.position = new Vector3(100,0,0);

        canvasMain = root.GetComponent<CanvasGroup>();
        
        controllerMenu = new CanvasController();
        controllerMenu.Init(root.transform.Find("Canvas - menu"));
        controllerHud = new CanvasController();
        controllerHud.Init(root.transform.Find("Canvas - hud"));
        controllerPopup = new CanvasController();
        controllerPopup.Init(root.transform.Find("Canvas - popup"));
        controllerEtc = new CanvasController();
        controllerEtc.Init(root.transform.Find("Canvas - etc"));

        cover = root.transform.Find("Image - dim");
        cover.gameObject.SetActive(false);

        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    public T OpenMenu<T>(string name) where T : MenuBase
    {
        T ret = controllerMenu.Open<T>(name);
        if (ret != null)
        {
            ret.OnInit();
        }
        
        return ret;
    }

    public T OpenHud<T>(string name) where T : HudBase
    {
        T ret = controllerPopup.Open<T>(name);
        if (ret != null)
        {
            ret.OnInit();
        }

        return ret;
    }

    public T OpenPopup<T>(string name) where T : PopupBase
    {
        T ret = controllerPopup.Open<T>(name);
        if (ret != null)
        {
            ret.OnInit();
        }

        CoverCheck();
        return ret;
    }

    public T OpenEtc<T>(string name) where T : UIObject
    {
        T ret = controllerPopup.Open<T>(name);
        if (ret != null)
        {
            ret.OnInit();
        }

        CoverCheck();
        return ret;
    }

    public void CloseMenu(string name)
    {
        controllerMenu.Close(name);
    }

    public void CloseHud(string name)
    {
        controllerHud.Close(name);
    }

    public void ClosePopup(string name)
    {
        controllerPopup.Close(name);
        CoverCheck();
    }

    public void CloseEtc(string name)
    {
        controllerEtc.Close(name);
        CoverCheck();
    }

    private void CoverCheck()
    {
        int countEtc = controllerEtc.Count();
        int countPopup = controllerPopup.Count();

        if(countEtc > 0)
        {
            cover.SetParent(controllerEtc.GetTransform());
            cover.SetSiblingIndex(countEtc - 1);
            cover.gameObject.SetActive(true);
        }
        else if(countPopup > 0)
        {
            cover.SetParent(controllerPopup.GetTransform());
            cover.SetSiblingIndex(countPopup - 1);
            cover.gameObject.SetActive(true);
        }
        else
        {
            cover.SetParent(canvasMain.transform);
            cover.SetSiblingIndex(0);
            cover.gameObject.SetActive(false);
        }
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
    private Canvas canvas = null;
    private CanvasGroup group = null;
    private List<UIObject> list = new List<UIObject>();

    public void Init(Transform trans)
    {
        canvas = trans.GetComponent<Canvas>();
        group = trans.GetComponent<CanvasGroup>();
    }

    public Transform GetTransform()
    {
        return canvas.transform;
    }

    public T Open<T>(string name) where T : UIObject
    {
        Transform trans = canvas.transform.Find(name);
        T ret = default(T);
        
        if(trans == null)
        {
            string path = string.Format("Prefabs/UI/{0}", name);
            T prefab = ResourcesManager.Instance.Load<T>(path);
            if (prefab == null)
            {
                Debug.Log($"{path} prefab is null.");
                return default(T);
            }

            T clone = GameObject.Instantiate<T>(prefab, canvas.transform);
            if (clone == null)
            {
                Debug.LogError($"clone is null.");
                return default(T);
            }

            clone.transform.SetAsLastSibling();
            clone.name = name;
            ret = clone;
        }
        else
        {
            ret = trans.GetComponent<T>();
        }

        if(list.Contains(ret) == false)
        {
            list.Add(ret);
        }

        ret.Init();
        return ret;
    }

    public void Close(string name)
    {
        var trans = canvas.transform.Find(name);
        if (trans != null)
        {
            UIObject obj = trans.GetComponent<UIObject>();
            obj.OnClose();

            if (list.Contains(obj) == true)
            {
                list.Remove(obj);
            }

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

    public T Get<T>(string name) where T : UIObject
    {
        var obj = canvas.transform.Find(name);
        return obj.GetComponent<T>();
    }

    public int Count()
    {
        return list.Count;
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
