using SweetSugar.Scriptable.Rewards;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.Localization;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.BonusSpin
{
    /// <summary>
    /// Reward on the wheel
    /// </summary>
    public class RewardWheel : MonoBehaviour
    {
        public RewardScriptable reward;
        public BoostType type;
        public int count;
        public string description;
        public int descriptionLocalizationRefrence;
        public string GetDescription()
        {
            return LocalizationManager.GetText(descriptionLocalizationRefrence, description);
        }

    }
}
