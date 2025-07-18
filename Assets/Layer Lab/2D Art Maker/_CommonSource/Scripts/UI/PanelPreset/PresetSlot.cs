using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LayerLab.ArtMaker
{
    public class PresetSlot : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] private PartsManager PartsManager { get; set; }
        [SerializeField] private bool useSave;
        private int _slotIndex;

        private readonly Color _defaultHairColor = new (0.5f, 0.5f, 0.5f);
        private readonly Color _defaultBeardColor = new (0.5f, 0.5f, 0.5f);
        private readonly Color _defaultBrowColor = new (0.5f, 0.5f, 0.5f);
        private readonly Color _defaultSkinColor = new (1f, 0.8f, 0.7f); // 기본 피부색
        /// <summary>
        /// 초기화
        /// Initialize
        /// </summary>
        /// <param name="index">인덱스 / Index</param>
        public void Init(int index)
        {
            _slotIndex = index;
            PartsManager = transform.GetComponentInChildren<PartsManager>();
            PartsManager.Init();
            LoadData();
        }

        /// <summary>
        /// 프리셋 데이터 로드
        /// Load preset data
        /// </summary>
        

        private void LoadData()
        {
            var dic = DemoControl.Instance.PresetData.LoadPreset(_slotIndex);
            if (dic != null && dic.Count > 0)
            {
                PartsManager.SetSkinActiveIndex(dic);
            }

            var colorData = DemoControl.Instance.PresetData.LoadPresetColors(_slotIndex);
            var positionData = DemoControl.Instance.PresetData.LoadPresetPositions(_slotIndex);

            if (colorData != null && colorData.Count > 0)
            {
                if (colorData.TryGetValue("hair", out Color hairColor))
                {
                    PartsManager.ChangeHairColor(hairColor);
                }
                else
                {
                    PartsManager.ChangeHairColor(_defaultHairColor);
                }

                if (colorData.TryGetValue("beard", out Color beardColor))
                {
                    PartsManager.ChangeBeardColor(beardColor);
                }
                else
                {
                    PartsManager.ChangeBeardColor(_defaultBeardColor);
                }

                if (colorData.TryGetValue("brow", out Color browColor))
                {
                    PartsManager.ChangeBrowColor(browColor);
                }
                else
                {
                    PartsManager.ChangeBrowColor(_defaultBrowColor);
                }

                if (colorData.TryGetValue("body", out Color skinColor))
                {
                    PartsManager.ChangeSkinColor(skinColor);
                }
                else
                {
                    PartsManager.ChangeSkinColor(_defaultSkinColor);
                }
            }
            else
            {
                PartsManager.ChangeHairColor(_defaultHairColor);
                PartsManager.ChangeBeardColor(_defaultBeardColor);
                PartsManager.ChangeBrowColor(_defaultBrowColor);
                PartsManager.ChangeSkinColor(_defaultSkinColor);
            }

            // 위치 데이터 적용
            if (positionData != null && positionData.Count > 0 && ColorPicker.Instance != null)
            {
                foreach (var kvp in positionData)
                {
                    ColorPicker.Instance.SetPartPosition(kvp.Key, kvp.Value);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    DemoControl.Instance.PanelParts.PanelPartsList.OnClick_Close();

                    ApplyAllPartsToCharacter();

                    var colorData = LoadColorDataFromPreset();
                    var positionData = LoadPositionDataFromPreset();

                    if (colorData != null && colorData.Count > 0)
                    {
                        if (colorData.TryGetValue("hair", out Color hairColor))
                        {
                            Player.Instance.PartsManager.ChangeHairColor(hairColor);
                            Player.Instance.PartsManager.OnColorChange.Invoke(PartsType.Hair_Short, hairColor);
                        }

                        if (colorData.TryGetValue("beard", out Color beardColor))
                        {
                            Player.Instance.PartsManager.ChangeBeardColor(beardColor);
                            Player.Instance.PartsManager.OnColorChange.Invoke(PartsType.Beard, beardColor);
                        }

                        if (colorData.TryGetValue("brow", out Color browColor))
                        {
                            Player.Instance.PartsManager.ChangeBrowColor(browColor);
                            Player.Instance.PartsManager.OnColorChange.Invoke(PartsType.Brow, browColor);
                        }

                        if (colorData.TryGetValue("body", out Color skinColor))
                        {
                            Player.Instance.PartsManager.ChangeSkinColor(skinColor);
                            Player.Instance.PartsManager.OnColorChange.Invoke(PartsType.Skin, skinColor);
                        }
                    }
                    else
                    {
                        Player.Instance.PartsManager.ChangeHairColor(PartsManager.GetColorBySlotType("hair"));
                        Player.Instance.PartsManager.ChangeBeardColor(PartsManager.GetColorBySlotType("beard"));
                        Player.Instance.PartsManager.ChangeBrowColor(PartsManager.GetColorBySlotType("brow"));
                        Player.Instance.PartsManager.ChangeSkinColor(PartsManager.GetColorBySlotType("body"));
                    }

                    if (positionData != null && positionData.Count > 0 && ColorPicker.Instance != null)
                    {
                        foreach (var kvp in positionData)
                        {
                            ColorPicker.Instance.SetPartPosition(kvp.Key, kvp.Value);
                        }
                    }

                    break;

                case PointerEventData.InputButton.Right:
                    if (!useSave) return;

                    ApplyAllPartsFromCharacterToPreset();

                    PartsManager.ChangeHairColor(Player.Instance.PartsManager.GetColorBySlotType("hair"));
                    PartsManager.ChangeBeardColor(Player.Instance.PartsManager.GetColorBySlotType("beard"));
                    PartsManager.ChangeBrowColor(Player.Instance.PartsManager.GetColorBySlotType("brow"));
                    PartsManager.ChangeSkinColor(Player.Instance.PartsManager.GetColorBySlotType("body"));

                    SavePresetData();
                    break;
            }
        }

        /// <summary>
        /// 프리셋에서 색상 데이터 로드
        /// Load color data from preset
        /// </summary>
        /// <returns>색상 데이터 / Color data</returns>
        private Dictionary<string, Color> LoadColorDataFromPreset()
        {
            return DemoControl.Instance.PresetData.LoadPresetColors(_slotIndex);
        }

        /// <summary>
        /// 프리셋에서 위치 데이터 로드
        /// Load position data from preset
        /// </summary>
        /// <returns>위치 데이터 / Position data</returns>
        private Dictionary<PartsType, Vector2> LoadPositionDataFromPreset()
        {
            return DemoControl.Instance.PresetData.LoadPresetPositions(_slotIndex);
        }

        /// <summary>
        /// 프리셋에서 캐릭터로 모든 부품 적용
        /// Apply all parts from preset to character
        /// </summary>
        private void ApplyAllPartsToCharacter()
        {
            Player.Instance.PartsManager.EquipParts(PartsType.Back, PartsManager.GetCurrentPartIndex(PartsType.Back));
            Player.Instance.PartsManager.EquipParts(PartsType.Beard, PartsManager.GetCurrentPartIndex(PartsType.Beard));
            Player.Instance.PartsManager.EquipParts(PartsType.Boots, PartsManager.GetCurrentPartIndex(PartsType.Boots));
            Player.Instance.PartsManager.EquipParts(PartsType.Bottom, PartsManager.GetCurrentPartIndex(PartsType.Bottom));
            Player.Instance.PartsManager.EquipParts(PartsType.Brow, PartsManager.GetCurrentPartIndex(PartsType.Brow));
            Player.Instance.PartsManager.EquipParts(PartsType.Eyes, PartsManager.GetCurrentPartIndex(PartsType.Eyes));
            Player.Instance.PartsManager.EquipParts(PartsType.Gloves, PartsManager.GetCurrentPartIndex(PartsType.Gloves));
            Player.Instance.PartsManager.EquipParts(PartsType.Hair_Short, PartsManager.GetCurrentPartIndex(PartsType.Hair_Short));
            Player.Instance.PartsManager.EquipParts(PartsType.Helmet, PartsManager.GetCurrentPartIndex(PartsType.Helmet));
            Player.Instance.PartsManager.EquipParts(PartsType.Mouth, PartsManager.GetCurrentPartIndex(PartsType.Mouth));
            Player.Instance.PartsManager.EquipParts(PartsType.Eyewear, PartsManager.GetCurrentPartIndex(PartsType.Eyewear));
            Player.Instance.PartsManager.EquipParts(PartsType.Gear_Left, PartsManager.GetCurrentPartIndex(PartsType.Gear_Left));
            Player.Instance.PartsManager.EquipParts(PartsType.Gear_Right, PartsManager.GetCurrentPartIndex(PartsType.Gear_Right));
            Player.Instance.PartsManager.EquipParts(PartsType.Top, PartsManager.GetCurrentPartIndex(PartsType.Top));
            Player.Instance.PartsManager.EquipParts(PartsType.Skin, PartsManager.GetCurrentPartIndex(PartsType.Skin));
        }

        /// <summary>
        /// 캐릭터에서 프리셋으로 모든 부품 적용
        /// Apply all parts from character to preset
        /// </summary>
        private void ApplyAllPartsFromCharacterToPreset()
        {
            PartsManager.EquipParts(PartsType.Back, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Back));
            PartsManager.EquipParts(PartsType.Beard, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Beard));
            PartsManager.EquipParts(PartsType.Boots, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Boots));
            PartsManager.EquipParts(PartsType.Bottom, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Bottom));
            PartsManager.EquipParts(PartsType.Brow, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Brow));
            PartsManager.EquipParts(PartsType.Eyes, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Eyes));
            PartsManager.EquipParts(PartsType.Gloves, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Gloves));
            PartsManager.EquipParts(PartsType.Hair_Short, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Hair_Short));
            PartsManager.EquipParts(PartsType.Helmet, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Helmet));
            PartsManager.EquipParts(PartsType.Mouth, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Mouth));
            PartsManager.EquipParts(PartsType.Eyewear, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Eyewear));
            PartsManager.EquipParts(PartsType.Gear_Left, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Gear_Left));
            PartsManager.EquipParts(PartsType.Gear_Right, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Gear_Right));
            PartsManager.EquipParts(PartsType.Top, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Top));
            PartsManager.EquipParts(PartsType.Skin, Player.Instance.PartsManager.GetCurrentPartIndex(PartsType.Skin));
        }

        /// <summary>
        /// 프리셋 저장
        /// Save preset data
        /// </summary>
        private void SavePresetData()
        {
            var colorData = new Dictionary<string, Color>();
            var hairColor = Player.Instance.PartsManager.GetColorBySlotType("hair");
            var beardColor = Player.Instance.PartsManager.GetColorBySlotType("beard");
            var browColor = Player.Instance.PartsManager.GetColorBySlotType("brow");
            var skinColor = Player.Instance.PartsManager.GetColorBySlotType("body");

            colorData.Add("hair", hairColor);
            colorData.Add("beard", beardColor);
            colorData.Add("brow", browColor);
            colorData.Add("body", skinColor);

            // 위치 데이터 수집
            var positionData = new Dictionary<PartsType, Vector2>();
            if (ColorPicker.Instance != null)
            {
                var hairPos = ColorPicker.Instance.GetPartPosition(PartsType.Hair_Short);
                var beardPos = ColorPicker.Instance.GetPartPosition(PartsType.Beard);
                var browPos = ColorPicker.Instance.GetPartPosition(PartsType.Brow);
                var skinPos = ColorPicker.Instance.GetPartPosition(PartsType.Skin);

                if (hairPos.x >= 0) positionData.Add(PartsType.Hair_Short, hairPos);
                if (beardPos.x >= 0) positionData.Add(PartsType.Beard, beardPos);
                if (browPos.x >= 0) positionData.Add(PartsType.Brow, browPos);
                if (skinPos.x >= 0) positionData.Add(PartsType.Skin, skinPos);
            }

            DemoControl.Instance.PresetData.SavePreset(_slotIndex, Player.Instance.PartsManager.ActiveIndices, colorData, positionData);
        }
    }
}