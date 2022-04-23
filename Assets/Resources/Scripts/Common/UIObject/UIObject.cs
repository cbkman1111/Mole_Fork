using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    protected Dictionary<string, GameObject> list = null;

    public void Init()
    {
        list = new Dictionary<string, GameObject>();

        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach(var child in children)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                if(list.ContainsKey(button.name) == false)
                {
                    button.onClick.AddListener(() => {
                        Click(button);
                    });

                    list.Add(button.name, button.gameObject);
                    continue;
                }
            }

            Slider slider = child.GetComponent<Slider>();
            if(slider != null)
            {
                if (list.ContainsKey(slider.name) == false)
                {
                    slider.value = 0;
                    slider.onValueChanged.AddListener((float f) =>
                    {
                        OnValueChanged(slider, f);
                    });

                    list.Add(slider.name, slider.gameObject);
                    continue;
                }
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                if(list.ContainsKey(text.name) == false)
                {
                    list.Add(text.name, text.gameObject);
                }

                continue;
            }
        }
    }

    public virtual void OnOpen(){}
    public virtual void OnClose(){}
    public virtual void OnValueChanged(Slider slider, float f){}

    protected int CompareTo(string a, string b)
    {
        return a.CompareTo(b);
    }

    protected T GetObject<T>(string name)
    {
        if (list.TryGetValue(name, out GameObject obj) == true)
        {
            return obj.GetComponent<T>();
        }
         
        return default(T);
    }

    protected void SetText(string name, string str)
    {
        if (list.TryGetValue(name, out GameObject obj) == true)
        {
            Text text = obj.GetComponent<Text>();
            if(text != null)
            {
                text.text = str;
            }
        }
    }

    protected void SetActive(string name, bool active)
    {
        if (list.TryGetValue(name, out GameObject obj) == true)
        {
            obj.gameObject.SetActive(active);
        }
    }

    private void Click(Button btn)
    {
        OnClick(btn);
    }

    public abstract void OnInit();
    protected abstract void OnClick(Button btn);
    public abstract void Close();

}
