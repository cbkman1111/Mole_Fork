using System;
using Common.Global;
using Common.Scene;
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
            for (int i = 0; i < 100; i++)
            {
                var obj = Instantiate<GameObject>(loadingCube);
                if(obj == null)
                {
                    continue;
                }

                var x = UnityEngine.Random.Range(-10f, 10f);
                var z = UnityEngine.Random.Range(-10f, 10f);
                var y = UnityEngine.Random.Range(0f, 5f);
                obj.transform.position = new Vector3(x, y, z);
                
                var renderer = obj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = MaterialLoadingCube;

                update(i / 100f);
            }
            update(1f);
        }

    }
}
