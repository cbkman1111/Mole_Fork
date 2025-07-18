using System;
using System.Collections;
using System.Collections.Generic;
using LayerLab.ArtMaker;
using Spine.Unity;
using UnityEngine;

namespace LayerLab.ArtMaker
{
   public class CharacterPrefabData : MonoBehaviour
   {
       #region Data Classes
       
       /// <summary>
       /// 스킨 부품 데이터 클래스
       /// Skin part data class
       /// </summary>
       [Serializable]
       public class SkinPartData
       {
           public PartsType partType;
           public int selectedIndex;
           public bool isHidden;
       }

       /// <summary>
       /// 슬롯 색상 데이터 클래스
       /// Slot color data class
       /// </summary>
       [Serializable]
       public class SlotColorData
       {
           public string slotName;
           public Color color;
       }

       /// <summary>
       /// 컬러피커 위치 데이터 클래스
       /// Color picker position data class
       /// </summary>
       [Serializable]
       public class ColorPickerPositionData
       {
           public PartsType partType;
           public Vector2 position;
       }
       
       #endregion

       #region Fields and Properties
       
       [SerializeField] public List<SkinPartData> skinParts = new();
       [SerializeField] public List<SlotColorData> slotColors = new();
       [SerializeField] public List<ColorPickerPositionData> colorPickerPositions = new();

       private PartsManager partsManager;
       
       #endregion

       #region Unity Lifecycle
       
       /// <summary>
       /// 컴포넌트 초기화
       /// Initialize component
       /// </summary>
       private void Awake()
       {
           partsManager = GetComponentInChildren<PartsManager>();
       }

       /// <summary>
       /// 시작 시 저장된 데이터 적용
       /// Apply saved data on start
       /// </summary>
       private void Start()
       {
           ApplySavedSkinData();
       }
       
       #endregion

       #region Data Loading
       
       /// <summary>
       /// 저장된 스킨 데이터 적용
       /// Apply saved skin data
       /// </summary>
       public void ApplySavedSkinData()
       {
           if (partsManager == null) return;

           partsManager.Init();
           ApplyPartData();
           ApplyColorData();
           ApplyColorPickerPositions();
       }

       /// <summary>
       /// 부품 데이터 적용
       /// Apply part data
       /// </summary>
       private void ApplyPartData()
       {
           var activeIndices = new Dictionary<PartsType, int>();

           foreach (var partData in skinParts)
           {
               activeIndices[partData.partType] = partData.selectedIndex;
               partsManager.SetHideItem(partData.partType, partData.isHidden);
           }

           partsManager.SetSkinActiveIndex(activeIndices);
       }

       /// <summary>
       /// 색상 데이터 적용
       /// Apply color data
       /// </summary>
       private void ApplyColorData()
       {
           foreach (var colorData in slotColors)
           {
               if (colorData.slotName.StartsWith("hair"))
               {
                   partsManager.ChangeHairColor(colorData.color);
               }
               else if (colorData.slotName.StartsWith("beard"))
               {
                   partsManager.ChangeBeardColor(colorData.color);
               }
               else if (colorData.slotName.StartsWith("brow"))
               {
                   partsManager.ChangeBrowColor(colorData.color);
               }
               else if (colorData.slotName.StartsWith("body"))
               {
                   partsManager.ChangeSkinColor(colorData.color);
               }
           }
       }

       /// <summary>
       /// 컬러피커 위치값 적용
       /// Apply color picker positions
       /// </summary>
       private void ApplyColorPickerPositions()
       {
           if (colorPickerPositions.Count > 0)
           {
               StartCoroutine(ApplyColorPickerPositionsDelayed());
           }
       }

       /// <summary>
       /// 컬러피커 위치값을 딜레이 후 적용
       /// Apply color picker positions after delay
       /// </summary>
       private IEnumerator ApplyColorPickerPositionsDelayed()
       {
           // ColorPicker 인스턴스가 초기화될 때까지 대기
           yield return new WaitForSeconds(0.1f);
           
           if (ColorPicker.Instance != null)
           {
               foreach (var positionData in colorPickerPositions)
               {
                   ColorPicker.Instance.SetPartPosition(positionData.partType, positionData.position);
                   Debug.Log($"Applied color picker position for {positionData.partType}: {positionData.position}");
               }
           }
           else
           {
               Debug.LogWarning("ColorPicker.Instance가 null입니다. 컬러피커 위치값을 적용할 수 없습니다.");
           }
       }
       
       #endregion

       #region Data Saving
       
       /// <summary>
       /// 현재 스킨 데이터를 컴포넌트에 저장
       /// Save current skin data to the component
       /// </summary>
       public void SaveCurrentSkinData()
       {
           if (partsManager == null) return;

           ClearAllData();
           SavePartData();
           SaveColorData();
           SaveColorPickerPositionData();
       }

       /// <summary>
       /// 모든 데이터 초기화
       /// Clear all data
       /// </summary>
       private void ClearAllData()
       {
           skinParts.Clear();
           slotColors.Clear();
           colorPickerPositions.Clear();
       }

       /// <summary>
       /// 부품 데이터 저장
       /// Save part data
       /// </summary>
       private void SavePartData()
       {
           foreach (var kvp in partsManager.ActiveIndices)
           {
               bool isHidden = IsPartHidden(kvp.Key);

               var partData = new SkinPartData
               {
                   partType = kvp.Key,
                   selectedIndex = kvp.Value,
                   isHidden = isHidden
               };
               skinParts.Add(partData);
           }
       }

       /// <summary>
       /// 색상 데이터 저장
       /// Save color data
       /// </summary>
       private void SaveColorData()
       {
           AddColorData("hair", partsManager.GetColorBySlotType("hair"));
           AddColorData("beard", partsManager.GetColorBySlotType("beard"));
           AddColorData("brow", partsManager.GetColorBySlotType("brow"));
           AddColorData("body", partsManager.GetColorBySlotType("body"));
       }

       /// <summary>
       /// 컬러피커 위치 데이터 저장
       /// Save color picker position data
       /// </summary>
       private void SaveColorPickerPositionData()
       {
           if (ColorPicker.Instance != null)
           {
               AddColorPickerPositionData(PartsType.Hair_Short);
               AddColorPickerPositionData(PartsType.Beard);
               AddColorPickerPositionData(PartsType.Brow);
               AddColorPickerPositionData(PartsType.Skin);
           }
       }
       
       #endregion

       #region Utility Methods
       
       /// <summary>
       /// 부품 숨김 상태 확인
       /// Check if part is hidden
       /// </summary>
       /// <param name="partType">부품 유형 / Part type</param>
       /// <returns>숨김 상태 / Hidden status</returns>
       private bool IsPartHidden(PartsType partType)
       {
           var hideStatusField = partsManager.GetType()
               .GetField("_hideStatus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

           if (hideStatusField != null)
           {
               var hideStatus = (Dictionary<PartsType, bool>)hideStatusField.GetValue(partsManager);
               return hideStatus.ContainsKey(partType) && hideStatus[partType];
           }

           return false;
       }

       /// <summary>
       /// 색상 데이터 추가
       /// Add color data
       /// </summary>
       /// <param name="slotPrefix">슬롯 접두사 / Slot prefix</param>
       /// <param name="color">색상 / Color</param>
       private void AddColorData(string slotPrefix, Color color)
       {
           slotColors.Add(new SlotColorData
           {
               slotName = slotPrefix,
               color = color
           });
       }

       /// <summary>
       /// 컬러피커 위치값 저장
       /// Save color picker position data
       /// </summary>
       /// <param name="partType">파츠 타입 / Part type</param>
       private void AddColorPickerPositionData(PartsType partType)
       {
           if (ColorPicker.Instance != null)
           {
               var position = ColorPicker.Instance.GetPartPosition(partType);
               if (position.x >= 0) // 유효한 위치값인 경우에만 저장
               {
                   colorPickerPositions.Add(new ColorPickerPositionData
                   {
                       partType = partType,
                       position = position
                   });
               }
           }
       }
       
       #endregion

       #region Public Accessors
       
       /// <summary>
       /// 저장된 컬러피커 위치값 가져오기
       /// Get saved color picker position
       /// </summary>
       /// <param name="partType">파츠 타입 / Part type</param>
       /// <returns>위치값 / Position value</returns>
       public Vector2 GetSavedColorPickerPosition(PartsType partType)
       {
           foreach (var positionData in colorPickerPositions)
           {
               if (positionData.partType == partType)
               {
                   return positionData.position;
               }
           }
           return new Vector2(-1, -1); // 저장된 위치가 없음을 나타냄
       }

       /// <summary>
       /// 컬러피커 위치값 설정
       /// Set color picker position
       /// </summary>
       /// <param name="partType">파츠 타입 / Part type</param>
       /// <param name="position">위치값 / Position value</param>
       public void SetColorPickerPosition(PartsType partType, Vector2 position)
       {
           // 기존 데이터 제거
           RemoveExistingPositionData(partType);

           // 새 데이터 추가
           AddNewPositionData(partType, position);
       }

       /// <summary>
       /// 기존 위치 데이터 제거
       /// Remove existing position data
       /// </summary>
       /// <param name="partType">파츠 타입 / Part type</param>
       private void RemoveExistingPositionData(PartsType partType)
       {
           for (int i = colorPickerPositions.Count - 1; i >= 0; i--)
           {
               if (colorPickerPositions[i].partType == partType)
               {
                   colorPickerPositions.RemoveAt(i);
               }
           }
       }

       /// <summary>
       /// 새 위치 데이터 추가
       /// Add new position data
       /// </summary>
       /// <param name="partType">파츠 타입 / Part type</param>
       /// <param name="position">위치값 / Position value</param>
       private void AddNewPositionData(PartsType partType, Vector2 position)
       {
           if (position.x >= 0)
           {
               colorPickerPositions.Add(new ColorPickerPositionData
               {
                   partType = partType,
                   position = position
               });
           }
       }
       
       #endregion
   }
}