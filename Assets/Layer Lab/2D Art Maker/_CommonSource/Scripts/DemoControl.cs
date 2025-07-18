using System;
using UnityEngine;
using UnityEngine.UI;

namespace LayerLab.ArtMaker
{
   public enum PlayMode
   {
       None,
       Home,
       Experience
   }

   public class DemoControl : MonoBehaviour
   {
       #region Fields and Properties
       
       public static DemoControl Instance { get; private set; }
       public Action<PlayMode> OnPlayMode { get; set; }
       public PlayMode CurrentPlayMode { get; set; } 

       [field: SerializeField] public PanelParts PanelParts { get; set; }
       [field: SerializeField] public PanelPreset PanelPreset { get; set; }
       [field: SerializeField] public PresetData PresetData { get; set; } // ScriptableObject 참조

       [SerializeField] private Sprite[] sprites;
       [SerializeField] private Button buttonHome, buttonRandomParts, buttonExperience;
       [SerializeField] private GameObject buttonMouseMove;
       public string pathAsset;
       
       #endregion

       #region Unity Lifecycle
       
       /// <summary>
       /// 인스턴스 설정
       /// Set instance
       /// </summary>
       private void Awake()
       {
           Instance = this;
       }

       /// <summary>
       /// 시작 시 초기화
       /// Initialize on start
       /// </summary>
       private void Start()
       {
           ChangeMode(PlayMode.Home);
           Init();
       }
       
       #endregion

       #region Initialization
       
       /// <summary>
       /// 초기화
       /// Initialize
       /// </summary>
       public void Init()
       {
           InitializeManagers();
           OnClick_RandomParts();
       }

       /// <summary>
       /// 매니저들 초기화
       /// Initialize managers
       /// </summary>
       private void InitializeManagers()
       {
           Player.Instance.PartsManager.Init();
           CameraControl.Instance.Init();
           Player.Instance.Init();
           PanelParts.Init();
           PanelPreset.Init();
           AnimationController.Instance.Init();
           
       }
       
       #endregion

       #region Static Methods
       
       /// <summary>
       /// 부품 유형별 색상 변경 가능 여부 확인
       /// Check if color can be changed for parts type
       /// </summary>
       /// <param name="partsType">부품 유형 / Parts type</param>
       /// <returns>색상 변경 가능 여부 / Can change color</returns>
       public static bool CanChangeColor(PartsType partsType) => 
           partsType is PartsType.Hair_Short or PartsType.Brow or PartsType.Beard or PartsType.Skin;
       
       #endregion

       #region Mode Management
       
       /// <summary>
       /// 플레이 모드 변경
       /// Change play mode
       /// </summary>
       /// <param name="playMode">플레이 모드 / Play mode</param>
       public void ChangeMode(PlayMode playMode)
       {
           if (CurrentPlayMode == playMode) return;
           CurrentPlayMode = playMode;
           OnPlayMode?.Invoke(playMode);

           switch (playMode)
           {
               case PlayMode.Home:
                   SetHomeMode();
                   break;
               case PlayMode.Experience:
                   SetExperienceMode();
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }
       }

       /// <summary>
       /// 홈 모드 UI 설정
       /// Set home mode UI
       /// </summary>
       private void SetHomeMode()
       {
           buttonMouseMove.SetActive(false);
           buttonRandomParts.gameObject.SetActive(true);
           buttonExperience.gameObject.SetActive(true);
           buttonHome.gameObject.SetActive(false);
       }

       /// <summary>
       /// 체험 모드 UI 설정
       /// Set experience mode UI
       /// </summary>
       private void SetExperienceMode()
       {
           buttonMouseMove.SetActive(true);
           buttonRandomParts.gameObject.SetActive(false);
           buttonExperience.gameObject.SetActive(false);
           buttonHome.gameObject.SetActive(true);
       }
       
       #endregion

       #region Utility Methods
       
       /// <summary>
       /// 스프라이트 가져오기
       /// Get sprite
       /// </summary>
       /// <param name="name">스프라이트 이름 / Sprite name</param>
       /// <returns>스프라이트 / Sprite</returns>
       public Sprite GetSprite(string name)
       {
           foreach (var t in sprites)
           {
               if (t.name == name.Split("/")[1]) return t;
           }

           return null;
       }
       
       #endregion

       #region Button Events
       
       /// <summary>
       /// 랜덤 부품 버튼 클릭
       /// Click random parts button
       /// </summary>
       public void OnClick_RandomParts()
       {
           Debug.Log("[DemoControl] OnClick_RandomParts");
           AudioManager.Instance.PlaySound(SoundList.ButtonRandom, 0.7f);
           PanelParts.PanelPartsList.OnClick_Close(false);
    
           // 먼저 파츠 랜덤 적용
           Player.Instance.PartsManager.RandomParts();
    
           // 그 다음 컬러피커에서 랜덤 색상 적용
           ColorPresetManager.Instance.SetRandomAllColor();
            
           // 랜덤 색상 적용 후 Hex 업데이트
           StartCoroutine(UpdateHexAfterRandomColors());
       }

       /// <summary>
       /// 랜덤 색상 적용 후 Hex 업데이트
       /// Update hex after applying random colors
       /// </summary>
       private System.Collections.IEnumerator UpdateHexAfterRandomColors()
       {
           yield return new WaitForEndOfFrame();
           yield return new WaitForEndOfFrame(); // 한 프레임 더 대기하여 모든 색상 적용 완료
            
           // 현재 선택된 파츠가 있다면 해당 파츠의 색상으로 Hex 업데이트
           if (ColorFavoriteManager.Instance != null && ColorPicker.Instance != null)
           {
               // 현재 파츠 타입 가져오기
               var currentPartsTypeField = typeof(ColorPicker).GetField("_currentPartsType", 
                   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
               if (currentPartsTypeField?.GetValue(ColorPicker.Instance) is PartsType currentPartsType 
                   && currentPartsType != PartsType.None)
               {
                   // 해당 파츠의 현재 색상으로 업데이트
                   Color currentColor = ColorPresetManager.Instance.GetColorByType(currentPartsType);
                   ColorFavoriteManager.Instance.UpdateHexDisplay(currentColor);
               }
           }
       }
       
       /// <summary>
       /// 체험하기 버튼 클릭
       /// Click experience button
       /// </summary>
       public void OnClick_Experience()
       {
           Player.Instance.SetCollider(true);
           AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
           ChangeMode(PlayMode.Experience);
       }

       /// <summary>
       /// 홈 버튼 클릭
       /// Click home button
       /// </summary>
       public void OnClick_Home()
       {
           Player.Instance.SetCollider(false);
           AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
           ChangeMode(PlayMode.Home);
       }
       
       #endregion

       #region SNS Button Events
       
       /// <summary>
       /// 디스코드 버튼 클릭
       /// Click Discord button
       /// </summary>
       public void OnClick_Discord()
       {
           
       }

       /// <summary>
       /// 페이스북 버튼 클릭
       /// Click Facebook button
       /// </summary>
       public void OnClick_Facebook()
       {
           
       }


       /// <summary>
       /// 에셋 스토어 버튼 클릭
       /// Click Asset Store button
       /// </summary>
       public void OnClick_AssetStore()
       {
           
       }

       /// <summary>
       /// 에셋 버튼 클릭
       /// Click Asset button
       /// </summary>
       public void OnClick_Asset()
       {
           
       }
       
       #endregion
   }
}