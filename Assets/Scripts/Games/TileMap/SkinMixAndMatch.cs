using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMixAndMatch : MonoBehaviour
{
    // character skins
    [SpineSkin] public string baseSkin = "A";

    // here we use arrays of strings to be able to cycle between them easily.
    [SpineSkin] public string[] headSkins = { "heads/hat_none", "heads/hat_a", "heads/hat_b", "heads/hat_c", "heads/hat_d"};
    public int activeHeadndex = 0;
    [SpineSkin] public string[] weaponeSkins = { "weapons/weapon_none", "weapons/weapon_a", "weapons/weapon_b" };
    public int activeWeaponeIndex = 0;
    
    Skin characterSkin;
    public SkeletonAnimation skeletonAnimation;

    public void NextHeadSkin()
    {
        activeHeadndex = (activeHeadndex + 1) % headSkins.Length;
        UpdateCharacterSkin();
        UpdateCombinedSkin();
    }

    public void NextWeaponeSkin()
    {
        activeWeaponeIndex = (activeWeaponeIndex + 1) % weaponeSkins.Length;
        UpdateCharacterSkin();
        UpdateCombinedSkin();
    }


    void UpdateCharacterSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        characterSkin = new Skin("character-base");
        // Note that the result Skin returned by calls to skeletonData.FindSkin()
        // could be cached once in Start() instead of searching for the same skin
        // every time. For demonstration purposes we keep it simple here.
        characterSkin.AddSkin(skeletonData.FindSkin(baseSkin));
        characterSkin.AddSkin(skeletonData.FindSkin(headSkins[activeHeadndex]));
        characterSkin.AddSkin(skeletonData.FindSkin(weaponeSkins[activeWeaponeIndex]));
    }

    void UpdateCombinedSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        Skin resultCombinedSkin = new Skin("character-combined");

        resultCombinedSkin.AddSkin(characterSkin);
        AddEquipmentSkinsTo(resultCombinedSkin);

        skeleton.SetSkin(resultCombinedSkin);
        skeleton.SetSlotsToSetupPose();
    }
    void AddEquipmentSkinsTo(Skin combinedSkin)
    {
        /*
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        combinedSkin.AddSkin(skeletonData.FindSkin(clothesSkin));
        combinedSkin.AddSkin(skeletonData.FindSkin(pantsSkin));
        if (!string.IsNullOrEmpty(bagSkin)) 
            combinedSkin.AddSkin(skeletonData.FindSkin(bagSkin));
        if (!string.IsNullOrEmpty(hatSkin)) 
            combinedSkin.AddSkin(skeletonData.FindSkin(hatSkin));
        */
    }
}
