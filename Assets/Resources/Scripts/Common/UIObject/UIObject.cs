using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    protected Dictionary<string, Button> buttons = null;
    protected Dictionary<string, Slider> sliders = null;
    protected Dictionary<string, Text> texts = null;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        buttons = new Dictionary<string, Button>();
        sliders = new Dictionary<string, Slider>();
        texts = new Dictionary<string, Text>();

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

                continue;
            }

            Slider slider = child.GetComponent<Slider>();
            if(slider != null)
            {
                sliders.Add(slider.name, slider);
                slider.value = 0;
                slider.onValueChanged.AddListener((float f) =>
                {
                    OnValueChanged(slider, f);
                });

                
                continue;
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                if(texts.ContainsKey(text.name) == false)
                {
                    texts.Add(text.name, text);
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
    public abstract void Close();

}
