using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuBundle : MenuBase
    {
        public Image test;
        public SpriteAtlas atlas = null;
        
        public override void OnInit()
        {

        }

        public bool InitMenu()
        {
           
            return true;
        }
        
        /*
        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += SpriteAtlasManagerOnAtlasRequested;
        }
 
        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= SpriteAtlasManagerOnAtlasRequested;
        }
        
        private void SpriteAtlasManagerOnAtlasRequested(string tag, Action<SpriteAtlas> arg2)
        {
            Debug.LogWarning(tag);
            
            var address = $"TestAtlas.spriteatlas";
            Debug.Log(address);
 
            var op = Addressables.LoadAssetAsync<Sprite>($"TestAtlas[mango]");
            Debug.Log(test.sprite.name);
            op.Completed += OpOnCompleted;
        }
        
        private void OpOnCompleted(AsyncOperationHandle<Sprite> op)
        {
            var sprite = op.Result;
            Debug.Log(sprite.name);
            test.sprite = sprite;
        }
        protected void OnValidate()
        {
            Debug.Log(test.sprite); // will stop .atlasRequested event from being fired
        }
        */
        
        void SpriteLocation(AsyncOperationHandle<IResourceLocation> obj)
        {  
            Addressables.LoadAssetAsync<Sprite>("").Completed += obj => test.sprite = obj.Result;

        }
        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - AdressBundle 2")
            {
                if (atlas == null)
                {
                    var loadAtals = Addressables.LoadAssetAsync<SpriteAtlas>("TestAtlas");
                    loadAtals.Completed += handle =>
                    {
                        switch (loadAtals.Status)
                        {
                            case AsyncOperationStatus.Succeeded:
                                atlas = loadAtals.Result;
                                //test.sprite = sprite;
                                //test.SetNativeSize();
                                
                                var sprite = atlas.GetSprite("mango");
                                test.sprite = sprite;
                                break;
                        
                            case AsyncOperationStatus.Failed:
                                break;

                            case AsyncOperationStatus.None:
                                break;
                        }
                    };
                }
                else
                {
                    var sprite = atlas.GetSprite("mango");
                    test.sprite = sprite;
                }

                
     
                /*
                var loadAtals = Addressables.LoadAssetAsync<SpriteAtlas>("TestAtlas.spriteatlas");
                loadAtals.Completed += handle =>
                {
                    var atlas = loadAtals.Result;
                    var sprite = atlas.GetSprite("mango");
                    test.sprite = sprite;
                };
      */
                
                string adress = "TestAddressable2";
                Addressables.InstantiateAsync(adress);
            }
            else if (name == "Button - Button - AseetBundle")
            {
                //
            }
        }
    }
}
