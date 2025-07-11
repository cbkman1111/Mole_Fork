using System;
using Common.Global;
using Common.Scene;
using Common.Utils;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneLoading : SceneBase
    {
        private UILoadingMenu menu = null;

        [SerializeField] private GameObject loadingCube = null;
        [SerializeField] private Material MaterialLoadingCube = null;


        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UILoadingMenu>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }

        public override async void Load(Action<float> update)
        {
            try
            {
                const int COUNT = 40000;
                for (int i = 0; i < COUNT; i++)
                {
                    var obj = Instantiate<GameObject>(loadingCube);
                    if (obj == null)
                    {
                        continue;
                    }

                    var x = UnityEngine.Random.Range(-50f, 50f);
                    var z = UnityEngine.Random.Range(-50f, 50f);
                    var y = UnityEngine.Random.Range(0f, 10f);
                    obj.transform.position = new Vector3(x, y, z);

                    var renderer = obj.GetComponent<MeshRenderer>();
                    renderer.sharedMaterial = MaterialLoadingCube;

                    update((float)i / (float)COUNT);
                }
            }
            catch (System.Exception e)
            {
                // handled below
                GiantDebug.LogError($"{name} - {e.ToString()}");
            }
           
            update(1f);
        }

    }
}
