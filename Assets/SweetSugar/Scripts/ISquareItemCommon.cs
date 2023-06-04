using UnityEngine;

namespace SweetSugar.Scripts
{
    public interface ISquareItemCommon
    {
        bool IsBottom();
        Sprite GetSprite();
        SpriteRenderer GetSpriteRenderer();
        SpriteRenderer[] GetSpriteRenderers();
    }
}