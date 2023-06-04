using System;
using UnityEngine;

namespace SweetSugar.Scripts.Localization
{
    [Serializable]
    public class LocalizationIndex
    {
        [Tooltip("Default text")]
        public string text;
        [Tooltip("Localization line index")]
        public int index;
    }
}