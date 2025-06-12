using System.Collections;
using System.Collections.Generic;
using Common.Global;
using Games.AntHouse.Objects;
using UnityEngine;

public class ObjectTemp : ObjectBase
{
    public SpriteRenderer sprite = null;
    protected override bool LoadSprite()
    {
        var prefab = ResourcesManager.Instance.LoadAddressable<SpriteRenderer>("ObjectSprite");
        sprite = Instantiate<SpriteRenderer>(prefab, transform);

        return true;
    }
}
