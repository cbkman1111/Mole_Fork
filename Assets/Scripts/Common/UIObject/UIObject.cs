using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UIObject
{
    public class PathAttribute : Attribute
    {
        public string ResourcePath { get; }

        public PathAttribute(string path)
        {
            ResourcePath = path;
        }
    }

    public class UIBase : MonoBehaviour
    {
        protected Dictionary<string, Component> List = null;

        private void Awake()
        {
            List = new Dictionary<string, Component>();

            var children = GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    if (List.ContainsKey(button.name) == false)
                    {
                        button.onClick.AddListener(() => {
                            OnClick(button);
                        });

                        List.Add(button.name, button);
                        continue;
                    }
                }

                var input = child.GetComponent<InputField>();
                if (input != null)
                {
                    if (List.ContainsKey(input.name) == false)
                    {
                        input.onValueChanged.AddListener((string str) =>
                        {
                            OnValueChanged(input, str);
                        });

                        List.Add(input.name, input);
                        continue;
                    }
                }

                var slider = child.GetComponent<Slider>();
                if (slider != null)
                {
                    if (List.ContainsKey(slider.name) == false)
                    {
                        slider.value = 0;
                        slider.onValueChanged.AddListener((float f) =>
                        {
                            OnValueChanged(slider, f);
                        });

                        List.Add(slider.name, slider);
                        continue;
                    }
                }

                var text = child.GetComponent<Text>();
                if (text != null)
                {
                    List.TryAdd(text.name, text);
                    continue;
                }

                var textPro = child.GetComponent<TextMeshProUGUI>();
                if (textPro != null)
                {
                    List.TryAdd(textPro.name, textPro);
                    continue;
                }

                var img = child.GetComponent<Image>();
                if (img != null)
                {
                    List.TryAdd(img.name, img);
                    continue;
                }

                var scroll = child.GetComponent<ScrollRect>();
                if (scroll != null)
                {
                    List.TryAdd(scroll.name, scroll);
                    continue;
                }
            }
        }

        protected int CompareTo(string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal);
        }

        protected T GetObject<T>(string key)
        {
            if (List.TryGetValue(key, out Component obj) == true)
            {
                return obj.GetComponent<T>();
            }

            return default(T);
        }

        protected void SetText(string key, string str)
        {
            if (List.TryGetValue(key, out Component obj) == true)
            {
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    text.text = str;
                }
            }
        }

        protected void SetTextMeshPro(string key, string str)
        {
            if (List.TryGetValue(key, out Component obj) == true)
            {
                TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = str;
                }
            }
        }

        protected void SetText(Text text, string str)
        {
            if (text != null)
            {
                text.text = str;
            }
        }

        protected void SetPosition(string key, Vector3 position)
        {
            if (List.TryGetValue(key, out Component obj) == true)
            {
                obj.transform.position = position;
            }
        }

        protected void SetActive(string key, bool active)
        {
            if (List.TryGetValue(key, out var obj) == true)
            {
                obj.gameObject.SetActive(active);
            }
        }

        protected void SetActive(Component component, bool active)
        {
            if (component == null)
                return;

            if(component.gameObject.activeSelf != active)
                component.gameObject.SetActive(active);
        }

        protected void SetSprite(string key, Sprite sprite)
        {
            if (List.TryGetValue(key, out var obj) != true) return;

            Image img = obj.GetComponent<Image>();
            if (img == true)
            {
                img.sprite = sprite;
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

        protected virtual void OnClick(Button btn) { }
        protected virtual void OnValueChanged(Slider slider, float f) { }
        protected virtual void OnValueChanged(InputField input, string str) { }
    }

    [Path("UI")]
    public abstract class UIObject : UIBase
    {
        protected override void OnClick(Button btn) { }
        protected override void OnValueChanged(Slider slider, float f) { }
        protected override void OnValueChanged(InputField input, string str) { }

        public virtual void OnOpen() { }
        public virtual void OnClose() { }
        public abstract void OnInit();
  
        public abstract void Close();
    }
}
