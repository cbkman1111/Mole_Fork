using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    protected Dictionary<string, Component> list = null;
    private void Awake()
    {
        list = new Dictionary<string, Component>();

        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                if (list.ContainsKey(button.name) == false)
                {
                    button.onClick.AddListener(() => {
                        Click(button);
                    });

                    list.Add(button.name, button);
                    continue;
                }
            }

            Slider slider = child.GetComponent<Slider>();
            if (slider != null)
            {
                if (list.ContainsKey(slider.name) == false)
                {
                    slider.value = 0;
                    slider.onValueChanged.AddListener((float f) =>
                    {
                        OnValueChanged(slider, f);
                    });

                    list.Add(slider.name, slider);
                    continue;
                }
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                if (list.ContainsKey(text.name) == false)
                {
                    list.Add(text.name, text);
                }

                continue;
            }

            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                if (list.ContainsKey(img.name) == false)
                {
                    list.Add(img.name, img);
                }

                continue;
            }
        }
    }

    protected int CompareTo(string a, string b)
    {
        return a.CompareTo(b);
    }

    protected T GetObject<T>(string key)
    {
        if (list.TryGetValue(key, out Component obj) == true)
        {
            return obj.GetComponent<T>();
        }
         
        return default(T);
    }
    
    protected void SetText(string key, string str)
    {
        if (list.TryGetValue(key, out Component obj) == true)
        {
            Text text = obj.GetComponent<Text>();
            if(text != null)
            {
                text.text = str;
            }
        }
    }

    protected void SetPosition(string key, Vector3 position)
    {
        if (list.TryGetValue(key, out Component obj) == true)
        {
            obj.transform.position = position;
        }
    }

    protected void SetActive(string key, bool active)
    {
        if (list.TryGetValue(key, out Component obj) == true)
        {
            obj.gameObject.SetActive(active);
        }
    }

    protected void SetSprite(string key, Sprite sprite)
    {
        if (list.TryGetValue(key, out Component obj) == true)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
            }
        }
    }

    public virtual Transform FindTransform(Transform node, string path)
    {
        if (node == false)
        {
            return null;
        }

        Transform trans = node;
        string[] names = path.Split('/');
        int index = 0;
        int count = names.Length;

        while (index < count)
        {
            trans = trans.Find(names[index]);
            if (trans)
            {
                index++;
            }
            else
            {
                trans = null;
                break;
            }
        }

        return trans;
    }

    private void Click(Button btn)
    {
        OnClick(btn);
    }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    public virtual void OnValueChanged(Slider slider, float f) { }
    public abstract void OnInit();
    protected abstract void OnClick(Button btn);
    public abstract void Close();

}
