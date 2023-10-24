using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class AddressableSpriteLoader : MonoBehaviour
{
    //private SpriteRenderer spriteRenderer;
    private Image _image = null;
    // AssetReference를 사용할 것인지 아니면 String으로 로드할 것인지 여부
    public bool isUseAddress;

    // 스프라이트 테스트 변수
    public AssetReferenceSprite newSprite;
    public string newSpriteAddress;
    void Start()
    {
        _image = GetComponent<Image>();
        _image.sprite = null;

        // isUseAddress가 true이면 string 값을 키값으로 사용해서 에셋을 로드합니다.
        if (isUseAddress == true)
        {
            Addressables.LoadAssetAsync<Sprite>(newSpriteAddress).Completed += SpriteLoaded;
        }

        // isUseAddress가 false이면 AssetReferenceSprite에 참조된 에셋을 로드합니다.
        else
        {
            newSprite.LoadAssetAsync().Completed += SpriteLoaded;
        }
    }
    
    private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
    {
         switch(obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                _image.sprite = obj.Result;
                break;

            case AsyncOperationStatus.Failed:
                Debug.Log("스프라이트 로드 실패");
                break;
            default:
                break;
        }
    }
}