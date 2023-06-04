using UnityEngine;

namespace SweetSugar.Scripts.Items._Interfaces
{
    public interface IColorEditor
    {
        Sprite[] Sprites { get; }
        Sprite randomEditorSprite { get; }
    }
}