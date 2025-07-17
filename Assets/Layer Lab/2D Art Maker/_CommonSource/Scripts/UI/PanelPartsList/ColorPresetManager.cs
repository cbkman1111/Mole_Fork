using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LayerLab.ArtMaker
{
    public class ColorPresetManager : MonoBehaviour
    {
        public static ColorPresetManager Instance { get; set; }
        
        private PartsType _currentPart;
        private Dictionary<PartsType, Color> _customColors = new ();
        
        private void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 타입 값으로 색상 가져오기
        /// Get color by part type
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>색상 / Color</returns>
        public Color GetColorByType(PartsType partsType)
        {
            if (_customColors.ContainsKey(partsType))
            {
                return _customColors[partsType];
            }

            // Player.Instance에서 실제 색상 가져오기
            if (Player.Instance?.PartsManager != null)
            {
                return GetActualColorFromPartsManager(partsType);
            }

            // 기본 색상 반환
            return GetDefaultColor(partsType);
        }
        
        /// <summary>
        /// PartsManager에서 실제 색상 가져오기
        /// Get actual color from PartsManager
        /// </summary>
        /// <param name="partsType">부품 유형</param>
        /// <returns>실제 색상</returns>
        private Color GetActualColorFromPartsManager(PartsType partsType)
        {
            try
            {
                return partsType switch
                {
                    PartsType.Hair_Short => Player.Instance.PartsManager.GetColorBySlotType("hair"),
                    PartsType.Beard => Player.Instance.PartsManager.GetColorBySlotType("beard"),
                    PartsType.Brow => Player.Instance.PartsManager.GetColorBySlotType("brow"),
                    PartsType.Skin => Player.Instance.PartsManager.GetColorBySlotType("body"),
                    _ => GetDefaultColor(partsType)
                };
            }
            catch
            {
                return GetDefaultColor(partsType);
            }
        }
        
        /// <summary>
        /// 기본 색상 가져오기
        /// Get default color
        /// </summary>
        /// <param name="partsType">부품 유형</param>
        /// <returns>기본 색상</returns>
        private Color GetDefaultColor(PartsType partsType)
        {
            return partsType switch
            {
                PartsType.Hair_Short => new Color(0.5f, 0.3f, 0.1f), // 갈색 머리
                PartsType.Beard => new Color(0.5f, 0.3f, 0.1f), // 갈색 수염
                PartsType.Brow => new Color(0.4f, 0.2f, 0.1f), // 어두운 갈색 눈썹
                PartsType.Skin => new Color(1f, 0.8f, 0.7f), // 기본 피부색
                _ => Color.white
            };
        }
    
        /// <summary>
        /// 타입별 색상 설정 (기존 SetPresetColor 대체)
        /// Set color by part type (replaces SetPresetColor)
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        public void SetPresetColor(PartsType partsType)
        {
            _currentPart = partsType;
            
            // 현재 파츠의 실제 색상 가져오기
            var currentColor = GetColorByType(partsType);
            _customColors[partsType] = currentColor;
            
            // 컬러피커에도 현재 파츠 타입 설정 (안전하게)
            if (ColorPicker.Instance != null)
            {
                ColorPicker.Instance.SetCurrentPartsType(partsType);
                
                // 컬러피커에서 해당 색상의 위치를 찾아서 설정
                SetColorPickerPositionByColor(partsType, currentColor);
                
                // ColorFavoriteManager의 Hex 업데이트도 트리거
                if (ColorFavoriteManager.Instance != null)
                {
                    // private 메서드를 호출하기 위해 리플렉션 사용
                    var updateHexMethod = typeof(ColorFavoriteManager).GetMethod("UpdateHexDisplay", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    updateHexMethod?.Invoke(ColorFavoriteManager.Instance, new object[] { currentColor });
                }
            }
            
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 색상에 따른 컬러피커 위치 설정
        /// Set color picker position by color
        /// </summary>
        /// <param name="partsType">파츠 타입</param>
        /// <param name="color">색상</param>
        private void SetColorPickerPositionByColor(PartsType partsType, Color color)
        {
            if (ColorPicker.Instance == null) return;
            
            // RGB를 HSV로 변환
            Color.RGBToHSV(color, out float h, out float s, out float v);
            
            // 컬러피커의 내부 상태 직접 업데이트
            var hueField = typeof(ColorPicker).GetField("_currentHue", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var saturationField = typeof(ColorPicker).GetField("_currentSaturation", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var valueField = typeof(ColorPicker).GetField("_currentValue", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var colorField = typeof(ColorPicker).GetField("_currentColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (hueField != null) hueField.SetValue(ColorPicker.Instance, h);
            if (saturationField != null) saturationField.SetValue(ColorPicker.Instance, s);
            if (valueField != null) valueField.SetValue(ColorPicker.Instance, v);
            if (colorField != null) colorField.SetValue(ColorPicker.Instance, color);
            
            // 파츠별 HSV와 색상 저장
            var partsHSVField = typeof(ColorPicker).GetField("_partsHSV", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var partsColorsField = typeof(ColorPicker).GetField("_partsColors", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (partsHSVField?.GetValue(ColorPicker.Instance) is Dictionary<PartsType, Vector3> partsHSV)
            {
                partsHSV[partsType] = new Vector3(h, s, v);
            }
            
            if (partsColorsField?.GetValue(ColorPicker.Instance) is Dictionary<PartsType, Color> partsColors)
            {
                partsColors[partsType] = color;
            }
            
            // 컬러피커 UI 업데이트
            var updateSVMethod = typeof(ColorPicker).GetMethod("UpdateSaturationValueTexture", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var updateIndicatorsMethod = typeof(ColorPicker).GetMethod("UpdateIndicators", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var updatePreviewMethod = typeof(ColorPicker).GetMethod("UpdatePreview", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            updateSVMethod?.Invoke(ColorPicker.Instance, null);
            updateIndicatorsMethod?.Invoke(ColorPicker.Instance, null);
            updatePreviewMethod?.Invoke(ColorPicker.Instance, null);
            
            Debug.Log($"Set color picker position for {partsType} with color {color} (H:{h:F3} S:{s:F3} V:{v:F3})");
        }

        /// <summary>
        /// 색상별 선택 설정
        /// Set selection by color
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="color">색상 / Color</param>
        public void SetSelectByColor(PartsType partsType, Color color)
        {
            _customColors[partsType] = color;
            // 여기서는 캐릭터 적용하지 않음 (다른 곳에서 이미 적용됨)
        }

        /// <summary>
        /// 모든 부품에 랜덤 색상 설정
        /// Set random colors for all parts
        /// </summary>
        public void SetRandomAllColor()
        {
            if (ColorPicker.Instance != null)
            {
                SetRandomColorFromPicker(PartsType.Hair_Short);
                SetRandomColorFromPicker(PartsType.Brow);
                SetRandomColorFromPicker(PartsType.Beard);
                SetRandomColorFromPicker(PartsType.Skin);
            }
            else
            {
                ApplyRandomColor(PartsType.Hair_Short);
                ApplyRandomColor(PartsType.Brow);
                ApplyRandomColor(PartsType.Beard);
                ApplyRandomColor(PartsType.Skin);
            }
        }
        
        /// <summary>
        /// 랜덤 색상 적용
        /// Apply random color
        /// </summary>
        /// <param name="partsType">부품 유형</param>
        private void ApplyRandomColor(PartsType partsType)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
            _customColors[partsType] = randomColor;
            
            switch (partsType)
            {
                case PartsType.Hair_Short:
                    Player.Instance.PartsManager.ChangeHairColor(randomColor);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Hair_Short, randomColor);
                    break;
            
                case PartsType.Beard:
                    Player.Instance.PartsManager.ChangeBeardColor(randomColor);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Beard, randomColor);
                    break;
            
                case PartsType.Brow:
                    Player.Instance.PartsManager.ChangeBrowColor(randomColor);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Brow, randomColor);
                    break;
            
                case PartsType.Skin:
                    Player.Instance.PartsManager.ChangeSkinColor(randomColor);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Skin, randomColor);
                    break;
            }
        }
        
        /// <summary>
        /// 컬러피커에서 랜덤 위치 색상 설정
        /// Set random position color from color picker
        /// </summary>
        /// <param name="partsType">부품 유형</param>
        private void SetRandomColorFromPicker(PartsType partsType)
        {
            Color randomColor = ColorPicker.Instance.ApplyRandomPositionToPart(partsType);
            _customColors[partsType] = randomColor;
            
            Debug.Log($"Applied random color to {partsType}: {ColorUtility.ToHtmlStringRGBA(randomColor)}");
        }
    }
}