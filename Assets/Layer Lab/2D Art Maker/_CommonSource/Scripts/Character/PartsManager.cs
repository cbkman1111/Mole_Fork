using System;
using System.Collections.Generic;
using System.Linq;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;
using AnimationState = Spine.AnimationState;
using Random = UnityEngine.Random;

namespace LayerLab.ArtMaker
{
    /// <summary>
    /// 부품 유형 열거형
    /// Parts type enumeration
    /// </summary>
    public enum PartsType
    {
        None,
        Back,
        Beard,
        Boots,
        Bottom,
        Brow,
        Eyes,
        Gloves,
        Hair_Short,
        Hair_Hat,
        Helmet,
        Mouth,
        Eyewear,
        Gear_Left,
        Gear_Right,
        Top,
        Skin,
    }

    /// <summary>
    /// 캐릭터 부품 관리자
    /// Character parts manager
    /// </summary>
    public class PartsManager : MonoBehaviour
    {
        #region Fields and Properties

        #region Spine Skin Lists
        [SpineSkin] public List<string> back = new(); // 등 부품 스킨 목록 / Back parts skin list
        [SpineSkin] public List<string> beard = new(); // 수염 부품 스킨 목록 / Beard parts skin list
        [SpineSkin] public List<string> boots = new(); // 신발 부품 스킨 목록 / Boots parts skin list
        [SpineSkin] public List<string> bottom = new(); // 하의 부품 스킨 목록 / Bottom parts skin list
        [SpineSkin] public List<string> brow = new(); // 눈썹 부품 스킨 목록 / Brow parts skin list
        [SpineSkin] public List<string> eye = new(); // 눈 부품 스킨 목록 / Eye parts skin list
        [SpineSkin] public List<string> gloves = new(); // 장갑 부품 스킨 목록 / Gloves parts skin list
        [SpineSkin] public List<string> hairShort = new(); // 짧은 머리 부품 스킨 목록 / Short hair parts skin list
        [SpineSkin] public List<string> hairHat = new(); // 헬멧용 머리 부품 스킨 목록 / Helmet hair parts skin list
        [SpineSkin] public List<string> helmet = new(); // 헬멧 부품 스킨 목록 / Helmet parts skin list
        [SpineSkin] public List<string> mouth = new(); // 입 부품 스킨 목록 / Mouth parts skin list
        [SpineSkin] public List<string> eyewear = new(); // 안경 부품 스킨 목록 / Eyewear parts skin list
        [SpineSkin] public List<string> gearLeft = new(); // 왼쪽 장비 부품 스킨 목록 / Left gear parts skin list
        [SpineSkin] public List<string> gearRight = new(); // 오른쪽 장비 부품 스킨 목록 / Right gear parts skin list
        [SpineSkin] public List<string> top = new(); // 상의 부품 스킨 목록 / Top parts skin list
        [SpineSkin] public List<string> skin = new(); // 피부 부품 스킨 목록 / Skin parts skin list
        #endregion

        #region Public Properties
        public Dictionary<PartsType, int> ActiveIndices { get; private set; } = new(); // 현재 활성화된 부품 인덱스 / Currently active parts index
        private Material runtimeMaterial; // 런타임 최적화용 머티리얼 / Runtime optimization material
        private Texture2D runtimeAtlas; // 런타임 최적화용 아틀라스 텍스처 / Runtime optimization atlas texture
        #endregion

        #region Events
        public Action<PartsType, int> OnChangedParts { get; set; } // 부품 변경 이벤트 / Parts change event
        public Action<PartsType, bool> OnPartsVisibilityChanged { get; set; } // 부품 가시성 변경 이벤트 / Parts visibility change event
        public Action<PartsType, Color> OnColorChange { get; set; } // 색상 변경 이벤트 / Color change event
        #endregion

        #region Private Fields
        private readonly Dictionary<PartsType, bool> _hideStatus = new(); // 각 부품의 숨김 상태 / Hide status of each part
        private Dictionary<PartsType, List<string>> _skinTypeToListMapping; // 부품 타입과 스킨 리스트 매핑 / Part type to skin list mapping
        private ISkeletonAnimation _skeletonComponent; // 스켈레톤 애니메이션 컴포넌트 인터페이스 / Skeleton animation component interface
        private Skeleton _skeleton; // 스파인 스켈레톤 객체 / Spine skeleton object
        private AnimationState _animationState; // 스파인 애니메이션 상태 / Spine animation state
        private Dictionary<string, Color> CustomSlotColors { get; } = new(); // 커스텀 슬롯 색상 / Custom slot colors
        private Skin _characterSkin; // 현재 캐릭터 스킨 / Current character skin
        private bool _hasCustomColors = false; // 커스텀 색상 적용 여부 / Whether custom colors are applied
        #endregion

        #region Serialized Fields
        [SerializeField] private SkeletonAnimation skeletonAnimation; // 스켈레톤 애니메이션 컴포넌트 / Skeleton animation component
        [SerializeField] private SkeletonGraphic skeletonGraphic; // 스켈레톤 그래픽 컴포넌트 / Skeleton graphic component
        [field: SerializeField] private bool IsOptimizeSkin { get; set; } // 스킨 최적화 활성화 여부 / Whether skin optimization is enabled
        [SerializeField] private bool autoInit; // 자동 초기화 활성화 여부 / Whether auto initialization is enabled
        #endregion

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// 에디터에서 스켈레톤 컴포넌트 자동 검색
        /// Auto find skeleton components in editor
        /// </summary>
        private void OnValidate()
        {
            if (skeletonAnimation == null && skeletonGraphic == null)
            {
                skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
                if (skeletonAnimation == null)
                {
                    skeletonGraphic = GetComponentInChildren<SkeletonGraphic>();
                }
            }
        }

        /// <summary>
        /// 시작 시 자동 초기화 확인
        /// Check auto initialization on start
        /// </summary>
        public void Start()
        {
            if (autoInit) Init();
        }

        /// <summary>
        /// 매 프레임 색상 적용 업데이트
        /// Update color application every frame
        /// </summary>
        private void LateUpdate()
        {
            if (_hasCustomColors && _animationState != null && _animationState.GetCurrent(0) != null)
            {
                ApplyCustomColors();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// 부품 매니저 초기화
        /// Initialize parts manager
        /// </summary>
        public void Init()
        {
            SetSkin();
            InitializeSkeletonComponents();
            InitializeMappings();
            InitializeSkinIndices();
            UpdateCharacter();
        }

        /// <summary>
        /// 스켈레톤 컴포넌트 초기화
        /// Initialize skeleton components
        /// </summary>
        private void InitializeSkeletonComponents()
        {
            if (skeletonAnimation != null)
            {
                _skeletonComponent = skeletonAnimation;
                _skeleton = skeletonAnimation.Skeleton;
                _animationState = skeletonAnimation.AnimationState;
            }
            else if (skeletonGraphic != null)
            {
                _skeletonComponent = skeletonGraphic;
                _skeleton = skeletonGraphic.Skeleton;
                _animationState = skeletonGraphic.AnimationState;
            }
            else
            {
                Debug.LogError("PartsManager에 SkeletonAnimation 또는 SkeletonGraphic이 없습니다!");
            }
        }

        /// <summary>
        /// 부품 유형과 스킨 리스트 매핑 초기화
        /// Initialize parts type to skin list mapping
        /// </summary>
        private void InitializeMappings()
        {
            // 스파인 매핑 재설정
            // Reset Spine Skin Type Mapping
            _skinTypeToListMapping = new Dictionary<PartsType, List<string>>()
            {
                { PartsType.Back, back },
                { PartsType.Beard, beard },
                { PartsType.Boots, boots },
                { PartsType.Bottom, bottom },
                { PartsType.Brow, brow },
                { PartsType.Eyes, eye },
                { PartsType.Gloves, gloves },
                { PartsType.Hair_Short, hairShort },
                { PartsType.Hair_Hat, hairHat },
                { PartsType.Helmet, helmet },
                { PartsType.Mouth, mouth },
                { PartsType.Eyewear, eyewear },
                { PartsType.Gear_Left, gearLeft },
                { PartsType.Gear_Right, gearRight },
                { PartsType.Top, top },
                { PartsType.Skin, skin }
            };

            InitializeHideStatus();
        }

        /// <summary>
        /// 숨김 상태 초기화
        /// Initialize hide status
        /// </summary>
        private void InitializeHideStatus()
        {
            foreach (PartsType partsType in Enum.GetValues(typeof(PartsType)))
            {
                if (partsType != PartsType.None)
                {
                    _hideStatus[partsType] = false;
                }
            }
        }

        /// <summary>
        /// 스킨 인덱스 초기화
        /// Initialize skin indices
        /// </summary>
        private void InitializeSkinIndices()
        {
            foreach (PartsType skinType in Enum.GetValues(typeof(PartsType)))
            {
                if (skinType != PartsType.None && _skinTypeToListMapping.ContainsKey(skinType))
                {
                    var skinList = _skinTypeToListMapping[skinType];
                    ActiveIndices[skinType] = skinList.Count > 0 ? 0 : -1;
                }
            }
        }

        #endregion

        #region Skin Data Management

        /// <summary>
        /// 스킨 데이터 설정
        /// Set skin data
        /// </summary>
        public void SetSkin()
        {
            InitializeSkeletonReference();

            if (_skeleton == null || _skeleton.Data == null)
            {
                Debug.LogError("No skeleton or skeleton data!");
                return;
            }

            LoadAllCategorySkins();
        }

        /// <summary>
        /// 스켈레톤 참조 초기화
        /// Initialize skeleton reference
        /// </summary>
        private void InitializeSkeletonReference()
        {
            if (skeletonAnimation)
            {
                _skeleton = skeletonAnimation.Skeleton;
            }

            if (skeletonGraphic)
            {
                _skeleton = skeletonGraphic.Skeleton;
            }
        }

        /// <summary>
        /// 모든 카테고리 스킨 로드
        /// Load all category skins
        /// </summary>
        private void LoadAllCategorySkins()
        {
            foreach (PartsType skinType in Enum.GetValues(typeof(PartsType)))
            {
                switch (skinType)
                {
                    case PartsType.None: break;
                    case PartsType.Back: back = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Beard: beard = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Boots: boots = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Bottom: bottom = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Brow: brow = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Eyes: eye = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Gloves: gloves = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Hair_Short: hairShort = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Hair_Hat: hairHat = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Helmet: helmet = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Mouth: mouth = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Eyewear: eyewear = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Gear_Left: gearLeft = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Gear_Right: gearRight = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Top: top = GetCategorySkins(_skeleton.Data, skinType); break;
                    case PartsType.Skin: skin = GetCategorySkins(_skeleton.Data, skinType); break;
                }
            }
        }

        /// <summary>
        /// 카테고리별 스킨 목록 가져오기
        /// Get list of skins by category
        /// </summary>
        /// <param name="data">스켈레톤 데이터 / Skeleton data</param>
        /// <param name="category">카테고리 / Category</param>
        /// <returns>스킨 목록 / Skin list</returns>
        private List<string> GetCategorySkins(SkeletonData data, PartsType category)
        {
            return (from s in data.Skins where s.Name.StartsWith($"{category.ToString().ToLower()}") select s.Name).ToList();
        }

        #endregion

        #region Parts Management

        /// <summary>
        /// 스킨 활성화 인덱스 설정
        /// Set skin activation index
        /// </summary>
        /// <param name="indexList">인덱스 목록 / Index list</param>
        public void SetSkinActiveIndex(Dictionary<PartsType, int> indexList)
        {
            if (indexList.Count <= 0) return;
            ActiveIndices = indexList;
            UpdateCharacter();
        }

        /// <summary>
        /// 부품 장착
        /// Equip part
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="index">부품 인덱스 / Part index</param>
        public void EquipParts(PartsType partsType, int index)
        {
            ActiveIndices[partsType] = index;
            HandleHelmetHairLogic(partsType, index);

            OnChangedParts?.Invoke(partsType, GetCurrentPartIndex(partsType));
            UpdateCharacter();
        }

        /// <summary>
        /// 헬멧 착용 시 머리카락 처리 로직
        /// Handle helmet hair logic
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="index">부품 인덱스 / Part index</param>
        private void HandleHelmetHairLogic(PartsType partsType, int index)
        {
            if (partsType == PartsType.Helmet)
            {
                if (index >= 0 && hairHat.Count > 0)
                    ActiveIndices[PartsType.Hair_Hat] = 0;
            }
        }

        /// <summary>
        /// 아이템 숨기기
        /// Hide item
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="isHide">숨김 여부 / Hide status</param>
        public void SetHideItem(PartsType partsType, bool isHide)
        {
            if (partsType != PartsType.None)
            {
                OnPartsVisibilityChanged?.Invoke(partsType, isHide);
                _hideStatus[partsType] = isHide;
                UpdateCharacter();
            }
        }

        /// <summary>
        /// 다음 아이템으로 변경
        /// Change to next item
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        public void NextItem(PartsType partsType)
        {
            if (!ValidatePartsType(partsType)) return;

            OnChangedParts?.Invoke(partsType, GetCurrentPartIndex(partsType));

            var skinList = _skinTypeToListMapping[partsType];
            if (skinList.Count == 0) return;

            UpdateIndexToNext(partsType, skinList);
            UpdateCharacter();
        }

        /// <summary>
        /// 이전 아이템으로 변경
        /// Change to previous item
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        public void PrevItem(PartsType partsType)
        {
            if (!ValidatePartsType(partsType)) return;

            OnChangedParts?.Invoke(partsType, GetCurrentPartIndex(partsType));

            var skinList = _skinTypeToListMapping[partsType];
            if (skinList.Count == 0) return;

            UpdateIndexToPrevious(partsType, skinList);
            UpdateCharacter();
        }

        /// <summary>
        /// 부품 유형 유효성 검사
        /// Validate parts type
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>유효성 / Validity</returns>
        private bool ValidatePartsType(PartsType partsType)
        {
            return partsType != PartsType.None && _skinTypeToListMapping.ContainsKey(partsType);
        }

        /// <summary>
        /// 인덱스를 다음으로 업데이트
        /// Update index to next
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="skinList">스킨 목록 / Skin list</param>
        private void UpdateIndexToNext(PartsType partsType, List<string> skinList)
        {
            var canHide = IsPartCanHide(partsType);

            if (canHide)
            {
                if (ActiveIndices[partsType] >= skinList.Count - 1)
                    ActiveIndices[partsType] = -1;
                else
                    ActiveIndices[partsType] += 1;
            }
            else
            {
                ActiveIndices[partsType] = (ActiveIndices[partsType] + 1) % skinList.Count;
            }
        }

        /// <summary>
        /// 인덱스를 이전으로 업데이트
        /// Update index to previous
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <param name="skinList">스킨 목록 / Skin list</param>
        private void UpdateIndexToPrevious(PartsType partsType, List<string> skinList)
        {
            bool canHide = IsPartCanHide(partsType);

            if (canHide)
            {
                if (ActiveIndices[partsType] <= -1)
                    ActiveIndices[partsType] = skinList.Count - 1;
                else
                    ActiveIndices[partsType] -= 1;
            }
            else
            {
                ActiveIndices[partsType] = (ActiveIndices[partsType] - 1 + skinList.Count) % skinList.Count;
            }
        }

        /// <summary>
        /// 랜덤 부품 생성
        /// Generate random parts
        /// </summary>
        public void RandomParts()
        {
            var allParts = DemoControl.Instance.PanelParts.partsSlots;

            for (int i = 0; i < allParts.Length; i++)
            {
                if (allParts[i].PartsType == PartsType.None) continue;

                int partsIndex = GenerateRandomPartsIndex(allParts[i]);
                ActiveIndices[allParts[i].PartsType] = partsIndex;
                OnChangedParts?.Invoke(allParts[i].PartsType, partsIndex);
            }

            UpdateCharacter();
        }

        /// <summary>
        /// 랜덤 부품 인덱스 생성
        /// Generate random parts index
        /// </summary>
        /// <param name="partsSlot">부품 슬롯 / Parts slot</param>
        /// <returns>부품 인덱스 / Parts index</returns>
        private int GenerateRandomPartsIndex(PartsSlot partsSlot)
        {
            var skinCount = GetCurrentSkinNames(partsSlot.PartsType).Count;

            if (partsSlot.CanHide)
            {
                return Random.value < 0.3f ? -1 : Random.Range(0, skinCount);
            }
            else
            {
                return Random.Range(0, skinCount);
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// 부품 유형이 빈 슬롯인지 확인
        /// Check if the part type is an empty slot
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>빈 슬롯 여부 / Empty slot status</returns>
        public bool IsEmptyItemByType(PartsType partsType)
        {
            return GetCurrentPartIndex(partsType) < 0 ||
                   (_skinTypeToListMapping.ContainsKey(partsType) && _skinTypeToListMapping[partsType].Count == 0);
        }

        /// <summary>
        /// 현재 부품 인덱스 반환
        /// Return the current part index
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>부품 인덱스 / Part index</returns>
        public int GetCurrentPartIndex(PartsType partsType)
        {
            return ActiveIndices.ContainsKey(partsType) ? ActiveIndices[partsType] : -1;
        }

        /// <summary>
        /// 현재 스킨 이름 목록 반환
        /// Return the current list of skin names
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>스킨 이름 목록 / Skin name list</returns>
        public List<string> GetCurrentSkinNames(PartsType partsType)
        {
            if (partsType == PartsType.None || !_skinTypeToListMapping.ContainsKey(partsType))
                return null;

            return _skinTypeToListMapping[partsType];
        }

        /// <summary>
        /// 현재 부품 이름 반환
        /// Return the current part name
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>부품 이름 / Part name</returns>
        public string GetCurrentPartsName(PartsType partsType)
        {
            if (partsType == PartsType.None || !_skinTypeToListMapping.ContainsKey(partsType))
                return string.Empty;

            var skinList = _skinTypeToListMapping[partsType];
            var index = ActiveIndices[partsType];

            return (index >= 0 && index < skinList.Count) ? skinList[index] : string.Empty;
        }

        /// <summary>
        /// 슬롯 유형별 색상 가져오기
        /// Get color by slot type
        /// </summary>
        /// <param name="slotName">슬롯 이름 / Slot name</param>
        /// <returns>색상 / Color</returns>
        public Color GetColorBySlotType(string slotName)
        {
            return CustomSlotColors[slotName];
        }

        /// <summary>
        /// 스켈레톤 가져오기
        /// Get skeleton
        /// </summary>
        /// <returns>스켈레톤 / Skeleton</returns>
        public Skeleton GetSkeleton() => skeletonAnimation.Skeleton;

        /// <summary>
        /// 스켈레톤 애니메이션 가져오기
        /// Get skeleton animation
        /// </summary>
        /// <returns>스켈레톤 애니메이션 / Skeleton animation</returns>
        public SkeletonAnimation GetSkeletonAnimation() => skeletonAnimation;

        #endregion

        #region Character Update

        /// <summary>
        /// 캐릭터 업데이트
        /// Update character
        /// </summary>
        public void UpdateCharacter()
        {
            UpdateCharacterSkin();
            UpdateCombinedSkin();
            if (IsOptimizeSkin) OptimizeSkin();
        }

        /// <summary>
        /// 캐릭터 스킨 업데이트
        /// Update character skin
        /// </summary>
        private void UpdateCharacterSkin()
        {
            if (_skeleton == null || _skeleton.Data == null) return;

            var skeletonData = _skeleton.Data;
            _characterSkin = new Skin("character-base");

            bool isHelmetEquipped = !IsEmptyItemByType(PartsType.Helmet) && !_hideStatus[PartsType.Helmet];
            HandleHairHatSynchronization();

            ApplyActiveSkins(skeletonData, isHelmetEquipped);
        }

        /// <summary>
        /// 헬멧 착용 시 머리 스타일 동기화
        /// Synchronize hair style when helmet is equipped
        /// </summary>
        private void HandleHairHatSynchronization()
        {
            ActiveIndices[PartsType.Hair_Hat] = ActiveIndices[PartsType.Hair_Short];
        }

        /// <summary>
        /// 활성 스킨 적용
        /// Apply active skins
        /// </summary>
        /// <param name="skeletonData">스켈레톤 데이터 / Skeleton data</param>
        /// <param name="isHelmetEquipped">헬멧 착용 여부 / Helmet equipped status</param>
        private void ApplyActiveSkins(SkeletonData skeletonData, bool isHelmetEquipped)
        {
            foreach (var skinType in _skinTypeToListMapping.Keys)
            {
                if (ShouldSkipSkinType(skinType, isHelmetEquipped)) continue;

                var index = ActiveIndices[skinType];
                var isHidden = _hideStatus[skinType];
                var skinList = _skinTypeToListMapping[skinType];

                if (index >= 0 && index < skinList.Count && !isHidden)
                {
                    ApplySkinToCharacter(skeletonData, skinList[index]);
                }
            }
        }

        /// <summary>
        /// 스킨 타입을 건너뛸지 확인
        /// Check if skin type should be skipped
        /// </summary>
        /// <param name="skinType">스킨 타입 / Skin type</param>
        /// <param name="isHelmetEquipped">헬멧 착용 여부 / Helmet equipped status</param>
        /// <returns>건너뛸지 여부 / Should skip</returns>
        private bool ShouldSkipSkinType(PartsType skinType, bool isHelmetEquipped)
        {
            if (skinType == PartsType.None) return true;
            if (skinType == PartsType.Hair_Short && isHelmetEquipped) return true;
            if (skinType == PartsType.Hair_Hat && !isHelmetEquipped) return true;

            return false;
        }

        /// <summary>
        /// 캐릭터에 스킨 적용
        /// Apply skin to character
        /// </summary>
        /// <param name="skeletonData">스켈레톤 데이터 / Skeleton data</param>
        /// <param name="skinName">스킨 이름 / Skin name</param>
        private void ApplySkinToCharacter(SkeletonData skeletonData, string skinName)
        {
            var skinData = skeletonData.FindSkin(skinName);
            if (skinData != null)
            {
                _characterSkin.AddSkin(skinData);
            }
        }

        /// <summary>
        /// 합성된 스킨 업데이트
        /// Update combined skin
        /// </summary>
        private void UpdateCombinedSkin()
        {
            if (_skeleton == null) return;

            Skin resultCombinedSkin = new("character-combined");
            resultCombinedSkin.AddSkin(_characterSkin);
            _skeleton.SetSkin(resultCombinedSkin);
            _skeleton.SetSlotsToSetupPose();

            if (_hasCustomColors)
            {
                ApplyCustomColors();
            }
        }

        /// <summary>
        /// 스켈레톤 업데이트
        /// Update skeleton
        /// </summary>
        private void UpdateSkeleton()
        {
            if (_skeleton == null || _animationState == null) return;

            _skeleton.SetSlotsToSetupPose();
            _animationState.Apply(_skeleton);
        }

        #endregion

        #region Color Management

        /// <summary>
        /// 커스텀 색상 적용
        /// Apply custom colors
        /// </summary>
        private void ApplyCustomColors()
        {
            if (_skeleton == null) return;

            foreach (var pair in CustomSlotColors)
            {
                var slot = _skeleton.FindSlot(pair.Key);
                if (slot != null)
                {
                    slot.SetColor(pair.Value);
                }
            }
        }

        /// <summary>
        /// 스킨 색상 변경
        /// Change skin color
        /// </summary>
        /// <param name="color">색상 / Color</param>
        public void ChangeSkinColor(Color color)
        {
            ApplyColorToSlotsWithPrefixes(color, new[] { "body", "leg_l", "leg_r", "arm_l", "arm_r", "head" });
            _hasCustomColors = true;
        }

        /// <summary>
        /// 머리카락 색상 변경
        /// Change hair color
        /// </summary>
        /// <param name="color">색상 / Color</param>
        public void ChangeHairColor(Color color)
        {
            ApplyColorToSlotsWithPrefixes(color, new[] { "hair", "helmet_hair" });
            _hasCustomColors = true;
        }

        /// <summary>
        /// 수염 색상 변경
        /// Change beard color
        /// </summary>
        /// <param name="color">색상 / Color</param>
        public void ChangeBeardColor(Color color)
        {
            ApplyColorToSlotsWithPrefixes(color, new[] { "beard" });
            _hasCustomColors = true;
        }

        /// <summary>
        /// 눈썹 색상 변경
        /// Change eyebrow color
        /// </summary>
        /// <param name="color">색상 / Color</param>
        public void ChangeBrowColor(Color color)
        {
            ApplyColorToSlotsWithPrefixes(color, new[] { "brow" });
            _hasCustomColors = true;
        }

        /// <summary>
        /// 접두사를 가진 슬롯들에 색상 적용
        /// Apply color to slots with prefixes
        /// </summary>
        /// <param name="color">색상 / Color</param>
        /// <param name="prefixes">접두사 배열 / Prefix array</param>
        private void ApplyColorToSlotsWithPrefixes(Color color, string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                var slots = GetSlotsWithPrefix(prefix);
                foreach (var slot in slots)
                {
                    string slotName = slot.Data.Name;
                    slot.SetColor(color);
                    CustomSlotColors[slotName] = color;
                }
            }
        }

        #endregion

        #region Animation

        /// <summary>
        /// 특정 이름의 애니메이션 재생
        /// Play an animation with a specific name
        /// </summary>
        /// <param name="animationName">애니메이션 이름 / Animation name</param>
        public void PlayAnimation(string animationName)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationName, true);
        }

        #endregion

        #region Optimization

        /// <summary>
        /// 스킨 최적화
        /// Optimize skin
        /// </summary>
        public void OptimizeSkin()
        {
            if (_skeleton == null || _skeletonComponent == null) return;

            var previousSkin = _skeleton.Skin;
            CleanupRuntimeResources();

            if (skeletonAnimation != null)
            {
                OptimizeSkeletonAnimation(previousSkin);
            }
            else if (skeletonGraphic != null)
            {
                OptimizeSkeletonGraphic(previousSkin);
            }
        }

        /// <summary>
        /// 런타임 리소스 정리
        /// Cleanup runtime resources
        /// </summary>
        private void CleanupRuntimeResources()
        {
            if (runtimeMaterial)
                Destroy(runtimeMaterial);
            if (runtimeAtlas)
                Destroy(runtimeAtlas);
        }

        /// <summary>
        /// 스켈레톤 애니메이션 최적화
        /// Optimize skeleton animation
        /// </summary>
        /// <param name="previousSkin">이전 스킨 / Previous skin</param>
        private void OptimizeSkeletonAnimation(Skin previousSkin)
        {
            var repackedSkin = previousSkin.GetRepackedSkin("Repacked skin",
                skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial,
                out runtimeMaterial, out runtimeAtlas);

            ApplyOptimizedSkin(previousSkin, repackedSkin);
        }

        /// <summary>
        /// 스켈레톤 그래픽 최적화
        /// Optimize skeleton graphic
        /// </summary>
        /// <param name="previousSkin">이전 스킨 / Previous skin</param>
        private void OptimizeSkeletonGraphic(Skin previousSkin)
        {
            var repackedSkin = previousSkin.GetRepackedSkin("Repacked skin",
                skeletonGraphic.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial,
                out runtimeMaterial, out runtimeAtlas);

            ApplyOptimizedSkin(previousSkin, repackedSkin);
        }

        /// <summary>
        /// 최적화된 스킨 적용
        /// Apply optimized skin
        /// </summary>
        /// <param name="previousSkin">이전 스킨 / Previous skin</param>
        /// <param name="repackedSkin">재패킹된 스킨 / Repacked skin</param>
        private void ApplyOptimizedSkin(Skin previousSkin, Skin repackedSkin)
        {
            previousSkin.Clear();
            _skeleton.Skin = repackedSkin;
            _skeleton.SetSlotsToSetupPose();
            _animationState.Apply(_skeleton);

            AtlasUtilities.ClearCache();
            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// 숨김 처리 가능 여부 확인
        /// Check if part can be hidden
        /// </summary>
        /// <param name="partsType">부품 유형 / Parts type</param>
        /// <returns>숨김 가능 여부 / Hideable status</returns>
        private static bool IsPartCanHide(PartsType partsType)
        {
            var partsSlots = DemoControl.Instance.PanelParts.partsSlots;
            foreach (var slot in partsSlots)
            {
                if (slot.PartsType == partsType)
                {
                    return slot.CanHide;
                }
            }

            return false;
        }

        /// <summary>
        /// 특정 접두사를 가진 슬롯 찾기
        /// Find slots with a specific prefix
        /// </summary>
        /// <param name="prefix">접두사 / Prefix</param>
        /// <returns>슬롯 목록 / Slot list</returns>
        private List<Slot> GetSlotsWithPrefix(string prefix)
        {
            if (_skeleton == null) return new List<Slot>();

            var result = new List<Slot>();
            foreach (var slot in _skeleton.Slots)
            {
                if (slot.Data.Name.ToLower().StartsWith(prefix.ToLower()))
                {
                    result.Add(slot);
                }
            }

            return result;
        }

        #endregion
    }
}