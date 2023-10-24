using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;


public class AddressableAtlasSpriteLoader : MonoBehaviour
{
    //private SpriteRenderer spriteRenderer;
    private Image _image = null;
    // AssetReference를 사용할 것인지 아니면 String으로 로드할 것인지 여부

    // 스프라이트 테스트 변수
    public AssetReferenceAtlasedSprite atalsSprite;
    
    void Start()
    {
        _image = GetComponent<Image>();
        _image.sprite = null;

        atalsSprite.LoadAssetAsync().Completed += SpriteLoaded;
        Addressables.LoadAssetAsync<Sprite>("TestAtlas[mango]").Completed += AtlasNameSubDone;
        //atalsSprite.LoadAssetAsync<SpriteAtlas>().Completed += SpriteAtlasLoaded;
    }
    
    void AtlasNameSubDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprite in atlas here.");
            return;
        }

        _image.sprite = op.Result;
    }
    
    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                Debug.Log($"스프라이트 포함 수량 : {obj.Result.spriteCount}");
                _image.sprite = obj.Result.GetSprite("mango"); 
                break;
            case AsyncOperationStatus.Failed: 
                Debug.LogError("Sprite load failed. Using default Sprite."); 
                break;
            default: // case AsyncOperationStatus.None: 
                break; 
        } 
    }
    private void SpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
         switch(handle.Status)
        {
            case AsyncOperationStatus.Succeeded:
                _image.sprite = handle.Result;
                break;

            case AsyncOperationStatus.Failed:
                Debug.Log("스프라이트 로드 실패");
                break;
            default:
                break;
        }
    }
}