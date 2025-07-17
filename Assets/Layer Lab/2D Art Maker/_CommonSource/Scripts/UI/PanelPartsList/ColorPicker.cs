using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LayerLab.ArtMaker
{
    public class ColorPicker : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [Header("HSV Color Picker Components")]
        [SerializeField] private RectTransform saturationValueArea;  // SV 영역 (사각형) / SV area (rectangle)
        [SerializeField] private RectTransform hueSlider;           // Hue 슬라이더 (스트립) / Hue slider (strip)
        [SerializeField] private RectTransform svIndicator;         // SV 영역의 원형 인디케이터 / Circular indicator for SV area
        [SerializeField] private RectTransform hueIndicator;        // Hue 슬라이더의 인디케이터 / Hue slider indicator
        [SerializeField] private Image saturationValueImage;        // SV 영역 이미지 / SV area image
        [SerializeField] private Image hueSliderImage;              // Hue 슬라이더 이미지 / Hue slider image
        [SerializeField] private Image previewImage;                // 선택된 색상 미리보기 / Selected color preview
        
        public static ColorPicker Instance { get; private set; }
        public Action<Color> OnColorChanged;
        
        private float _currentHue = 0f;        // 0-1 범위 / 0-1 range
        private float _currentSaturation = 1f; // 0-1 범위 / 0-1 range
        private float _currentValue = 1f;      // 0-1 범위 / 0-1 range
        private Color _currentColor = Color.red;
        private PartsType _currentPartsType;
        private bool _isInitialized = false;
        
        // 각 파츠별로 HSV 값 저장 / Store HSV values for each part
        private Dictionary<PartsType, Vector3> _partsHSV = new Dictionary<PartsType, Vector3>();
        private Dictionary<PartsType, Color> _partsColors = new Dictionary<PartsType, Color>();
        
        // 드래그 상태 추적 / Track drag state
        private bool _isDraggingSV = false;
        private bool _isDraggingHue = false;
        
        // 1. 필드 추가 (기존 필드들 아래에)
        [Header("Favorite Colors & Hex")]
        [SerializeField] private ColorFavoriteManager favoriteManager;
        
        private void Awake()
        {
            Instance = this;
        }
        
        // 2. Start() 메서드에 추가
        private void Start()
        {
            // 기존 코드...
            SetupHueSliderHandler();
    
            // 즐겨찾기 매니저 초기화 추가
            if (favoriteManager != null)
            {
                favoriteManager.Init(this);
            }
        }
        
        /// <summary>
        /// 컬러피커 초기화
        /// Initialize color picker
        /// </summary>
        public void InitializeColorTexture()
        {
            if (_isInitialized) return;
            
            InitializeHSVColorPicker();
            _isInitialized = true;
            Debug.Log("HSV ColorPicker initialized successfully");
        }
        
        /// <summary>
        /// HSV 컬러피커 초기화
        /// Initialize HSV color picker
        /// </summary>
        private void InitializeHSVColorPicker()
        {
            CreateHueSliderTexture();
            UpdateSaturationValueTexture();
            UpdateIndicators();
        }
        
        /// <summary>
        /// Hue 슬라이더 텍스처 생성
        /// Create hue slider texture
        /// </summary>
        private void CreateHueSliderTexture()
        {
            if (hueSliderImage == null) return;
            
            int width = 256;
            int height = 1;
            Texture2D hueTexture = new Texture2D(width, height);
            
            for (int x = 0; x < width; x++)
            {
                float hue = (float)x / (width - 1);
                Color color = Color.HSVToRGB(hue, 1f, 1f);
                hueTexture.SetPixel(x, 0, color);
            }
            
            hueTexture.Apply();
            hueTexture.wrapMode = TextureWrapMode.Clamp;
            hueSliderImage.sprite = Sprite.Create(hueTexture, new Rect(0, 0, width, height), Vector2.one * 0.5f);
        }
        
        /// <summary>
        /// Saturation-Value 영역 텍스처 업데이트
        /// Update saturation-value area texture
        /// </summary>
        private void UpdateSaturationValueTexture()
        {
            if (saturationValueImage == null) return;
            
            int size = 256;
            Texture2D svTexture = new Texture2D(size, size);
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float saturation = (float)x / (size - 1);
                    float value = (float)y / (size - 1);
                    Color color = Color.HSVToRGB(_currentHue, saturation, value);
                    svTexture.SetPixel(x, y, color);
                }
            }
            
            svTexture.Apply();
            saturationValueImage.sprite = Sprite.Create(svTexture, new Rect(0, 0, size, size), Vector2.one * 0.5f);
        }
        
        // 3. 컬러피커 열기 메서드 수정
        public void OpenColorPicker(PartsType partsType, Color currentColor, Action<Color> onColorSelected)
        {
            // 기존 코드...
            if (!_isInitialized)
            {
                InitializeColorTexture();
            }
    
            _currentPartsType = partsType;
            OnColorChanged = onColorSelected;
    
            gameObject.SetActive(true);
    
            // 한 프레임 대기 후 활성화
            StartCoroutine(ActivateColorPickerDelayed(partsType, currentColor));
        }
        
        /// <summary>
        /// 컬러피커 활성화 지연 실행
        /// Delayed activation of color picker
        /// </summary>
        private IEnumerator ActivateColorPickerDelayed(PartsType partsType, Color currentColor)
        {
            yield return null;
            
            // 저장된 HSV 값이 있으면 사용, 없으면 현재 색상에서 변환
            if (_partsHSV.ContainsKey(partsType))
            {
                Vector3 hsv = _partsHSV[partsType];
                _currentHue = hsv.x;
                _currentSaturation = hsv.y;
                _currentValue = hsv.z;
                Debug.Log($"Using saved HSV for {partsType}: {hsv}");
            }
            else
            {
                Color.RGBToHSV(currentColor, out _currentHue, out _currentSaturation, out _currentValue);
                Debug.Log($"Converting RGB to HSV for {partsType}: H:{_currentHue:F3} S:{_currentSaturation:F3} V:{_currentValue:F3}");
            }
            
            _currentColor = Color.HSVToRGB(_currentHue, _currentSaturation, _currentValue);
            
            UpdateSaturationValueTexture();
            UpdateIndicators();
            UpdatePreview();
            
            Debug.Log($"ColorPicker fully activated for {partsType}");
        }
 
        
        /// <summary>
        /// 포인터 다운 이벤트
        /// Pointer down event
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isInitialized)
            {
                return;
            }
            
            Debug.Log("OnPointerDown called");
            Vector2 localPoint;
            bool foundTarget = false;
            
            // SV 영역 클릭 확인
            if (saturationValueArea != null)
            {
                bool isInSV = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    saturationValueArea, eventData.position, eventData.pressEventCamera, out localPoint);
                
                Debug.Log($"SV Area check - Screen pos: {eventData.position}, Local pos: {localPoint}, IsIn: {isInSV}");
                
                if (isInSV && IsPointInRect(localPoint, saturationValueArea.rect))
                {
                    _isDraggingSV = true;
                    foundTarget = true;
                    UpdateSaturationValue(localPoint);
                    Debug.Log("Started dragging SV area");
                }
            }
            
            // Hue 슬라이더 클릭 확인 (SV 영역이 아닐 때만)
            if (!foundTarget && hueSlider != null)
            {
                bool isInHue = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    hueSlider, eventData.position, eventData.pressEventCamera, out localPoint);
                
                Debug.Log($"Hue Slider check - Screen pos: {eventData.position}, Local pos: {localPoint}, IsIn: {isInHue}");
                Debug.Log($"Hue Slider rect: {hueSlider.rect}");
                
                if (isInHue && IsPointInRect(localPoint, hueSlider.rect))
                {
                    _isDraggingHue = true;
                    foundTarget = true;
                    UpdateHue(localPoint);
                    Debug.Log("Started dragging Hue slider");
                }
            }
            
            if (!foundTarget)
            {
                Debug.Log("No valid target found for click");
            }
        }
        
        /// <summary>
        /// 드래그 이벤트
        /// Drag event
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isInitialized)
            {
                return;
            }
            
            Vector2 localPoint;
            
            if (_isDraggingSV && saturationValueArea != null)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    saturationValueArea, eventData.position, eventData.pressEventCamera, out localPoint))
                {
                    UpdateSaturationValue(localPoint);
                }
            }
            else if (_isDraggingHue && hueSlider != null)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    hueSlider, eventData.position, eventData.pressEventCamera, out localPoint))
                {
                    UpdateHue(localPoint);
                }
            }
        }
        
        /// <summary>
        /// 점이 Rect 내부에 있는지 확인
        /// Check if point is inside rect
        /// </summary>
        private bool IsPointInRect(Vector2 localPoint, Rect rect)
        {
            return localPoint.x >= rect.xMin && localPoint.x <= rect.xMax &&
                   localPoint.y >= rect.yMin && localPoint.y <= rect.yMax;
        }
        
        /// <summary>
        /// Saturation과 Value 업데이트
        /// Update saturation and value
        /// </summary>
        private void UpdateSaturationValue(Vector2 localPoint)
        {
            var rect = saturationValueArea.rect;
            
            _currentSaturation = Mathf.Clamp01((localPoint.x - rect.x) / rect.width);
            _currentValue = Mathf.Clamp01((localPoint.y - rect.y) / rect.height);
            
            UpdateColor();
            UpdateIndicators();
        }
        
        /// <summary>
        /// Hue 업데이트
        /// Update hue
        /// </summary>
        private void UpdateHue(Vector2 localPoint)
        {
            if (hueSlider == null) return;
            
            var rect = hueSlider.rect;
            Debug.Log($"UpdateHue - LocalPoint: {localPoint}, Rect: {rect}");
            
            float newHue;
            
            // Hue 슬라이더가 수직인지 수평인지 확인
            if (rect.width > rect.height) // 수평 슬라이더
            {
                newHue = Mathf.Clamp01((localPoint.x - rect.x) / rect.width);
                Debug.Log($"Horizontal slider - X: {localPoint.x}, Rect.x: {rect.x}, Width: {rect.width}, NewHue: {newHue}");
            }
            else // 수직 슬라이더
            {
                newHue = Mathf.Clamp01((localPoint.y - rect.y) / rect.height);
                Debug.Log($"Vertical slider - Y: {localPoint.y}, Rect.y: {rect.y}, Height: {rect.height}, NewHue: {newHue}");
            }
            
            _currentHue = newHue;
            Debug.Log($"Hue updated to: {_currentHue}");
            
            UpdateColor();
            UpdateSaturationValueTexture(); // Hue가 변경되면 SV 영역도 업데이트
            UpdateIndicators();
        }
        
        /// <summary>
        /// 색상 업데이트 및 적용
        /// Update and apply color
        /// </summary>
        private void UpdateColor()
        {
            _currentColor = Color.HSVToRGB(_currentHue, _currentSaturation, _currentValue);
            
            // 현재 파츠의 HSV와 색상 저장
            _partsHSV[_currentPartsType] = new Vector3(_currentHue, _currentSaturation, _currentValue);
            _partsColors[_currentPartsType] = _currentColor;
            
            OnColorChanged?.Invoke(_currentColor);
            ApplyColorToCharacter(_currentColor);
            UpdatePreview();
            
            Debug.Log($"Color updated - H:{_currentHue:F3} S:{_currentSaturation:F3} V:{_currentValue:F3} RGB:{_currentColor}");
        }
        
        /// <summary>
        /// 인디케이터 위치 업데이트
        /// Update indicator positions
        /// </summary>
        private void UpdateIndicators()
        {
            // SV 인디케이터 위치 업데이트
            if (svIndicator != null && saturationValueArea != null)
            {
                var svRect = saturationValueArea.rect;
                float x = svRect.x + (svRect.width * _currentSaturation);
                float y = svRect.y + (svRect.height * _currentValue);
                svIndicator.localPosition = new Vector3(x, y, 0);
                Debug.Log($"SV Indicator updated to: {svIndicator.localPosition}");
            }
            
            // Hue 인디케이터 위치 업데이트
            if (hueIndicator != null && hueSlider != null)
            {
                var hueRect = hueSlider.rect;
                Vector3 newPosition;
                
                if (hueRect.width > hueRect.height) // 수평 슬라이더
                {
                    float x = hueRect.x + (hueRect.width * _currentHue);
                    newPosition = new Vector3(x, hueRect.center.y, 0);
                }
                else // 수직 슬라이더
                {
                    float y = hueRect.y + (hueRect.height * _currentHue);
                    newPosition = new Vector3(hueRect.center.x, y, 0);
                }
                
                hueIndicator.localPosition = newPosition;
                Debug.Log($"Hue Indicator updated to: {hueIndicator.localPosition} (Hue: {_currentHue})");
            }
            else
            {
                Debug.LogWarning($"Hue indicator components missing - hueIndicator: {hueIndicator}, hueSlider: {hueSlider}");
            }
        }
        
        /// <summary>
        /// 색상 미리보기 업데이트
        /// Update color preview
        /// </summary>
        private void UpdatePreview()
        {
            if (previewImage != null)
            {
                previewImage.color = _currentColor;
            }
        }
        
        /// <summary>
        /// 캐릭터에 색상 적용
        /// Apply color to character
        /// </summary>
        private void ApplyColorToCharacter(Color color)
        {
            if (Player.Instance?.PartsManager == null) return;

            switch (_currentPartsType)
            {
                case PartsType.Hair_Short:
                    Player.Instance.PartsManager.ChangeHairColor(color);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Hair_Short, color);
                    break;
                
                case PartsType.Beard:
                    Player.Instance.PartsManager.ChangeBeardColor(color);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Beard, color);
                    break;
                
                case PartsType.Brow:
                    Player.Instance.PartsManager.ChangeBrowColor(color);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Brow, color);
                    break;
                
                case PartsType.Skin:
                    Player.Instance.PartsManager.ChangeSkinColor(color);
                    Player.Instance.PartsManager.OnColorChange?.Invoke(PartsType.Skin, color);
                    break;
                
                default:
                    Debug.LogWarning($"Unknown parts type: {_currentPartsType}");
                    break;
            }
        }
        
        /// <summary>
        /// 현재 색상 가져오기
        /// Get current color
        /// </summary>
        public Color GetCurrentColor()
        {
            return _currentColor;
        }
        
        /// <summary>
        /// 현재 파츠의 위치값 가져오기 (레거시 호환성)
        /// Get current part's position value (legacy compatibility)
        /// </summary>
        public Vector2 GetPartPosition(PartsType partsType)
        {
            if (_partsHSV.ContainsKey(partsType))
            {
                // HSV 값을 2D 위치로 변환 (S=x, V=y)
                Vector3 hsv = _partsHSV[partsType];
                return new Vector2(hsv.y, hsv.z); // Saturation, Value
            }
            return new Vector2(-1, -1);
        }
        
        /// <summary>
        /// 특정 파츠의 위치값 설정 (레거시 호환성)
        /// Set specific part's position value (legacy compatibility)
        /// </summary>
        public void SetPartPosition(PartsType partsType, Vector2 position)
        {
            if (position.x >= 0 && position.y >= 0)
            {
                // 기존 Hue 값 유지하면서 Saturation과 Value만 설정
                float existingHue = _partsHSV.ContainsKey(partsType) ? _partsHSV[partsType].x : 0f;
                Vector3 hsv = new Vector3(existingHue, position.x, position.y);
                
                Color colorAtPosition = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
                _partsHSV[partsType] = hsv;
                _partsColors[partsType] = colorAtPosition;
                
                if (_currentPartsType == partsType)
                {
                    _currentHue = hsv.x;
                    _currentSaturation = hsv.y;
                    _currentValue = hsv.z;
                    _currentColor = colorAtPosition;
                    
                    UpdateSaturationValueTexture();
                    UpdateIndicators();
                    UpdatePreview();
                }
            }
        }
        
        /// <summary>
        /// 현재 파츠 타입 설정
        /// Set current parts type
        /// </summary>
        public void SetCurrentPartsType(PartsType partsType)
        {
            _currentPartsType = partsType;
            
            if (_partsHSV.ContainsKey(partsType))
            {
                Vector3 hsv = _partsHSV[partsType];
                _currentHue = hsv.x;
                _currentSaturation = hsv.y;
                _currentValue = hsv.z;
                _currentColor = Color.HSVToRGB(_currentHue, _currentSaturation, _currentValue);
               
                UpdateSaturationValueTexture();
                UpdateIndicators();
                UpdatePreview();
            }
        }
        
        /// <summary>
        /// 랜덤 위치로 포인터 이동 및 색상 적용
        /// Move pointer to random position and apply color
        /// </summary>
        public Color SetRandomPosition()
        {
            _currentHue = UnityEngine.Random.Range(0f, 1f);
            _currentSaturation = UnityEngine.Random.Range(0.5f, 1f);
            _currentValue = UnityEngine.Random.Range(0.5f, 1f);
            
            UpdateColor();
            
            UpdateSaturationValueTexture();
            UpdateIndicators();
            
            return _currentColor;
        }
        
        /// <summary>
        /// 특정 파츠 타입에 대한 랜덤 위치 색상 적용
        /// Apply random position color for specific parts type
        /// </summary>
        public Color SetRandomPositionForPart(PartsType partsType)
        {
            float randomHue = UnityEngine.Random.Range(0f, 1f);
            float randomSaturation = UnityEngine.Random.Range(0.5f, 1f);
            float randomValue = UnityEngine.Random.Range(0.5f, 1f);
            
            Color randomColor = Color.HSVToRGB(randomHue, randomSaturation, randomValue);
            
            _partsHSV[partsType] = new Vector3(randomHue, randomSaturation, randomValue);
            _partsColors[partsType] = randomColor;
            
            if (_currentPartsType == partsType)
            {
                _currentHue = randomHue;
                _currentSaturation = randomSaturation;
                _currentValue = randomValue;
                _currentColor = randomColor;
                
                UpdateSaturationValueTexture();
                UpdateIndicators();
                UpdatePreview();
            }
            
            return randomColor;
        }
        
        /// <summary>
        /// 특정 파츠 타입에 대한 랜덤 위치 색상 적용 (캐릭터에 바로 적용)
        /// Apply random position color for specific parts type (directly to character)
        /// </summary>
        public Color ApplyRandomPositionToPart(PartsType partsType)
        {
            Color randomColor = SetRandomPositionForPart(partsType);
            
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
            
            return randomColor;
        }
        
        /// <summary>
        /// 수동으로 인디케이터 위치 설정 (레거시 호환성)
        /// Manually set indicator position (legacy compatibility)
        /// </summary>
        public void SetIndicatorPosition(Vector2 normalizedPosition)
        {
            if (_currentPartsType != PartsType.None)
            {
                // normalizedPosition을 HSV로 변환 (x=Saturation, y=Value)
                float existingHue = _partsHSV.ContainsKey(_currentPartsType) ? _partsHSV[_currentPartsType].x : 0f;
                
                _currentHue = existingHue;
                _currentSaturation = Mathf.Clamp01(normalizedPosition.x);
                _currentValue = Mathf.Clamp01(normalizedPosition.y);
                
                UpdateColor();
                UpdateIndicators();
            }
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _isDraggingSV = false;
                _isDraggingHue = false;
            }
        }

        
        /// <summary>
        /// HueSliderHandler 설정
        /// Setup HueSliderHandler
        /// </summary>
        private void SetupHueSliderHandler()
        {
            if (hueSlider != null)
            {
                var hueHandler = hueSlider.GetComponent<HueSliderHandler>();
                if (hueHandler == null)
                {
                    hueHandler = hueSlider.gameObject.AddComponent<HueSliderHandler>();
                }
                
                hueHandler.OnHueChanged += OnHueChangedFromSlider;
                Debug.Log("HueSliderHandler connected to ColorPicker");
            }
        }
        
        /// <summary>
        /// Hue 슬라이더에서 Hue 값이 변경되었을 때 호출
        /// Called when hue value is changed from hue slider
        /// </summary>
        private void OnHueChangedFromSlider(float newHue)
        {
            _currentHue = newHue;
            Debug.Log($"ColorPicker: Hue updated from slider to {_currentHue}");
            
            UpdateColor();
            UpdateSaturationValueTexture(); // Hue가 변경되면 SV 영역도 업데이트
            UpdateIndicators();
        }
        
        /// <summary>
        /// 메모리 정리
        /// Memory cleanup
        /// </summary>
        private void OnDestroy()
        {
            if (saturationValueImage?.sprite?.texture != null)
            {
                Destroy(saturationValueImage.sprite.texture);
            }
            if (hueSliderImage?.sprite?.texture != null)
            {
                Destroy(hueSliderImage.sprite.texture);
            }
        }
    }
}