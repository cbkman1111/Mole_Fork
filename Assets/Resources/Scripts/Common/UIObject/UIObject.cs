using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    protected Dictionary<string, Button> buttons = null;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        buttons = new Dictionary<string, Button>();
        if(buttons == null)
        {
            Debug.LogError("dictionaryButton can not be null.");
        }

        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach(var child in children)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                buttons.Add(button.name, button);
                button.onClick.AddListener(() => {
                    Click(button);  
                });
            }
        }
    }

    protected int CompareTo(string a, string b)
    {
        return a.CompareTo(b);
    }

    protected Button GetButton(string name)
    {
        if(buttons.ContainsKey(name) == true)
        {
            return buttons[name];
        }

        return null;
    }

    private void Click(Button btn)
    {
        OnClick(btn);
    }

    public abstract void OnClick(Button btn);
}
