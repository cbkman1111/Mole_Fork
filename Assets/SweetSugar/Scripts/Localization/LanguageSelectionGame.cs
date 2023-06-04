using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.Localization;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LanguageSelectionGame : UnityEngine.MonoBehaviour
    {
        private List<CultureInfo> cultures;
        private TMP_Dropdown Dropdown;
        public CurtureTuple[] extraLanguages;
        private void Start()
        {
            Dropdown = GetComponent<TMP_Dropdown>();
            var txt = Resources.LoadAll<TextAsset>("Localization/");
            cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(i => txt.Any(x => x.name == i.DisplayName)).ToList();
            cultures.AddRange(extraLanguages.Select(i=>new CultureInfo(i.culture)));
            Dropdown.options = cultures.Select(i => new TMP_Dropdown.OptionData(i.Name.ToUpper(), null)).ToList();
            var _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
            Dropdown.captionText.text = cultures.First(i => i.EnglishName == LocalizationManager.GetSystemLanguage(_debugSettings).ToString()).Name.ToUpper();
            Dropdown.value = cultures.ToList().FindIndex(i => i.EnglishName == LocalizationManager.GetSystemLanguage(_debugSettings).ToString());
        }

        public void OnChangeLanguage()
        {
            LocalizationManager.LoadLanguage(GetSelectedLanguage().EnglishName);
            CrosssceneData.selectedLanguage = GetSelectedLanguage().EnglishName;
        }

        private CultureInfo GetSelectedLanguage()
        {
            return cultures.ToArray()[Dropdown.value];
        }
    }

    [Serializable]
    public struct CurtureTuple
    {
        public string culture;
        public string name;
    }
}