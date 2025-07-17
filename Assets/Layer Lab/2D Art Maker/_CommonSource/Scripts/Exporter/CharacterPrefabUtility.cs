using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace LayerLab.ArtMaker
{
#if UNITY_EDITOR
    public class CharacterPrefabUtility : MonoBehaviour
    {
        public static CharacterPrefabUtility Instance;
        
        #region Fields and Properties

        [Header("Thumbnail Settings")] [SerializeField]
        private int thumbnailWidth = 512; // 썸네일 너비 / Thumbnail width

        [SerializeField] private int thumbnailHeight = 512; // 썸네일 높이 / Thumbnail height
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -10); // 카메라 오프셋 / Camera offset
        [SerializeField] private float fixedCameraSize = 3f; // 고정 카메라 크기 / Fixed camera size
        [SerializeField] private LayerMask characterLayerMask = -1; // 캐릭터 레이어 마스크 / Character layer mask
        [SerializeField] private bool useFixedPosition = true; // 고정 위치 사용 여부 / Use fixed position

        #endregion


        #region Private Methods
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        #region Public Methods
        
        /// <summary>
        /// 독립적인 캐릭터 프리팹 생성
        /// Create self-contained character prefab
        /// </summary>
        [ContextMenu("Create Self-Contained Character Prefab")]
        public void CreateCharacterPrefab()
        {
            // 플레이어 인스턴스 및 PartsManager 유효성 검사
            // Validate Player instance and PartsManager
            if (!ValidatePlayerInstance(out var characterObj, out var sourcePartsManager))
                return;

            // 프리팹 오브젝트 생성 및 초기화
            // Create and initialize prefab object
            var prefabObj = CreatePrefabObject(characterObj);
            var prefabData = InitializePrefabData(prefabObj);

            // 부품 데이터 수집 및 저장
            // Collect and save parts data
            CollectPartsData(sourcePartsManager, prefabData);

            // 색상 데이터 수집 및 저장
            // Collect and save color data
            CollectColorData(sourcePartsManager, prefabData);

            // 컬러피커 위치 데이터 수집 및 저장
            // Collect and save color picker position data
            CollectColorPickerPositions(prefabData);

            // 썸네일 생성을 위한 환경 설정
            // Setup environment for thumbnail capture
            SetupThumbnailEnvironment(prefabObj, out var map);

            // 썸네일 캡처
            // Capture thumbnail
            var thumbnailTexture = CaptureCharacterThumbnail(characterObj);

            // 환경 복원
            // Restore environment
            RestoreThumbnailEnvironment(prefabObj, map);

            // 프리팹 에셋 저장
            // Save prefab asset
            SavePrefabAsset(prefabObj, prefabData, thumbnailTexture);
        }

        #endregion

        #region Validation

        /// <summary>
        /// 플레이어 인스턴스 유효성 검사
        /// Validate player instance
        /// </summary>
        /// <param name="characterObj">캐릭터 오브젝트 / Character object</param>
        /// <param name="sourcePartsManager">소스 PartsManager / Source PartsManager</param>
        /// <returns>유효성 여부 / Validation result</returns>
        private bool ValidatePlayerInstance(out GameObject characterObj, out PartsManager sourcePartsManager)
        {
            characterObj = null;
            sourcePartsManager = null;

            if (Player.Instance?.PartsManager?.gameObject == null)
            {
                Debug.LogError("Character Instance not found.");
                return false;
            }

            characterObj = Player.Instance.PartsManager.gameObject;
            sourcePartsManager = Player.Instance.PartsManager;

            if (sourcePartsManager.ActiveIndices == null || sourcePartsManager.ActiveIndices.Count == 0)
            {
                Debug.LogError("No active indices found on the source PartsManager.");
                return false;
            }

            return true;
        }

        #endregion

        #region Prefab Creation

        /// <summary>
        /// 프리팹 오브젝트 생성
        /// Create prefab object
        /// </summary>
        /// <param name="characterObj">원본 캐릭터 오브젝트 / Original character object</param>
        /// <returns>생성된 프리팹 오브젝트 / Created prefab object</returns>
        private GameObject CreatePrefabObject(GameObject characterObj)
        {
            var prefabObj = Instantiate(characterObj);
            prefabObj.name = characterObj.name + "_Prefab";
            return prefabObj;
        }

        /// <summary>
        /// 프리팹 데이터 컴포넌트 초기화
        /// Initialize prefab data component
        /// </summary>
        /// <param name="prefabObj">프리팹 오브젝트 / Prefab object</param>
        /// <returns>CharacterPrefabData 컴포넌트 / CharacterPrefabData component</returns>
        private CharacterPrefabData InitializePrefabData(GameObject prefabObj)
        {
            var prefabData = prefabObj.GetComponent<CharacterPrefabData>();
            if (prefabData == null)
            {
                prefabData = prefabObj.AddComponent<CharacterPrefabData>();
            }

            prefabData.skinParts.Clear();
            prefabData.slotColors.Clear();
            prefabData.colorPickerPositions.Clear();

            return prefabData;
        }

        #endregion

        #region Data Collection

        /// <summary>
        /// 부품 데이터 수집
        /// Collect parts data
        /// </summary>
        /// <param name="sourcePartsManager">소스 PartsManager / Source PartsManager</param>
        /// <param name="prefabData">프리팹 데이터 / Prefab data</param>
        private void CollectPartsData(PartsManager sourcePartsManager, CharacterPrefabData prefabData)
        {
            Debug.Log($"Source ActiveIndices count: {sourcePartsManager.ActiveIndices.Count}");

            foreach (PartsType partType in System.Enum.GetValues(typeof(PartsType)))
            {
                if (partType == PartsType.None) continue;

                if (sourcePartsManager.ActiveIndices.ContainsKey(partType))
                {
                    int index = sourcePartsManager.ActiveIndices[partType];
                    Debug.Log($"Adding part: {partType} with index: {index}");

                    var partData = new CharacterPrefabData.SkinPartData
                    {
                        partType = partType,
                        selectedIndex = index,
                        isHidden = false
                    };
                    prefabData.skinParts.Add(partData);
                }
            }
        }

        /// <summary>
        /// 색상 데이터 수집
        /// Collect color data
        /// </summary>
        /// <param name="sourcePartsManager">소스 PartsManager / Source PartsManager</param>
        /// <param name="prefabData">프리팹 데이터 / Prefab data</param>
        private void CollectColorData(PartsManager sourcePartsManager, CharacterPrefabData prefabData)
        {
            try
            {
                var hairColor = sourcePartsManager.GetColorBySlotType("hair");
                var beardColor = sourcePartsManager.GetColorBySlotType("beard");
                var browColor = sourcePartsManager.GetColorBySlotType("brow");
                var bodyColor = sourcePartsManager.GetColorBySlotType("body");

                Debug.Log($"Hair color: {hairColor}, Beard color: {beardColor}, Brow color: {browColor}, Skin color: {bodyColor}");

                // 색상 데이터를 프리팹에 추가
                // Add color data to prefab
                AddColorData(prefabData, "hair", hairColor);
                AddColorData(prefabData, "beard", beardColor);
                AddColorData(prefabData, "brow", browColor);
                AddColorData(prefabData, "body", bodyColor);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting colors: {e.Message}");
            }
        }

        /// <summary>
        /// 색상 데이터 추가 헬퍼 메서드
        /// Helper method to add color data
        /// </summary>
        /// <param name="prefabData">프리팹 데이터 / Prefab data</param>
        /// <param name="slotName">슬롯 이름 / Slot name</param>
        /// <param name="color">색상 / Color</param>
        private void AddColorData(CharacterPrefabData prefabData, string slotName, Color color)
        {
            prefabData.slotColors.Add(new CharacterPrefabData.SlotColorData
            {
                slotName = slotName,
                color = color
            });
        }

        /// <summary>
        /// 컬러피커 위치 데이터 수집
        /// Collect color picker position data
        /// </summary>
        /// <param name="prefabData">프리팹 데이터 / Prefab data</param>
        private void CollectColorPickerPositions(CharacterPrefabData prefabData)
        {
            try
            {
                if (ColorPicker.Instance == null)
                {
                    Debug.LogWarning("ColorPicker.Instance가 null입니다. 컬러피커 위치값을 저장할 수 없습니다.");
                    return;
                }

                // 각 파츠의 컬러피커 위치값 수집
                // Collect color picker position values for each part
                var positions = new[]
                {
                    (PartsType.Hair_Short, ColorPicker.Instance.GetPartPosition(PartsType.Hair_Short)),
                    (PartsType.Beard, ColorPicker.Instance.GetPartPosition(PartsType.Beard)),
                    (PartsType.Brow, ColorPicker.Instance.GetPartPosition(PartsType.Brow)),
                    (PartsType.Skin, ColorPicker.Instance.GetPartPosition(PartsType.Skin))
                };

                Debug.Log($"Hair position: {positions[0].Item2}, Beard position: {positions[1].Item2}, " +
                          $"Brow position: {positions[2].Item2}, Skin position: {positions[3].Item2}");

                // 유효한 위치값만 저장
                // Save only valid position values
                foreach (var (partType, position) in positions)
                {
                    if (position.x >= 0)
                    {
                        prefabData.colorPickerPositions.Add(new CharacterPrefabData.ColorPickerPositionData
                        {
                            partType = partType,
                            position = position
                        });
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting color picker positions: {e.Message}");
            }
        }

        #endregion

        #region Thumbnail Environment

        /// <summary>
        /// 썸네일 촬영을 위한 환경 설정
        /// Setup environment for thumbnail capture
        /// </summary>
        /// <param name="prefabObj">프리팹 오브젝트 / Prefab object</param>
        /// <param name="map">맵 오브젝트 / Map object</param>
        private void SetupThumbnailEnvironment(GameObject prefabObj, out GameObject map)
        {
            // 프리팹 오브젝트 비활성화 (썸네일에 나타나지 않게)
            // Deactivate prefab object (so it doesn't appear in thumbnail)
            prefabObj.SetActive(false);

            // 맵 오브젝트 찾아서 비활성화 (배경 제거)
            // Find and deactivate map object (remove background)
            map = GameObject.Find("Map");
            if (map != null)
            {
                map.SetActive(false);
            }
        }

        /// <summary>
        /// 썸네일 촬영 후 환경 복원
        /// Restore environment after thumbnail capture
        /// </summary>
        /// <param name="prefabObj">프리팹 오브젝트 / Prefab object</param>
        /// <param name="map">맵 오브젝트 / Map object</param>
        private void RestoreThumbnailEnvironment(GameObject prefabObj, GameObject map)
        {
            // 프리팹 오브젝트 다시 활성화
            // Reactivate prefab object
            prefabObj.SetActive(true);

            // 맵 오브젝트 다시 활성화
            // Reactivate map object
            if (map != null)
            {
                map.SetActive(true);
            }
        }

        #endregion

        #region Thumbnail Capture

        /// <summary>
        /// 캐릭터 썸네일 캡처 (배경 투명)
        /// Capture character thumbnail with transparent background
        /// </summary>
        /// <param name="characterObj">캐릭터 오브젝트 / Character object</param>
        /// <returns>캡처된 텍스처 / Captured texture</returns>
        private Texture2D CaptureCharacterThumbnail(GameObject characterObj)
        {
            // 임시 카메라 생성 및 설정
            // Create and setup temporary camera
            var tempCameraObj = new GameObject("ThumbnailCamera");
            var tempCamera = SetupThumbnailCamera(tempCameraObj, characterObj);

            // 렌더 텍스처 생성 및 렌더링
            // Create render texture and render
            var renderTexture = CreateRenderTexture();
            tempCamera.targetTexture = renderTexture;
            tempCamera.Render();

            // 텍스처로 변환
            // Convert to texture
            var thumbnail = ConvertRenderTextureToTexture2D(renderTexture);

            // 리소스 정리
            // Cleanup resources
            CleanupThumbnailResources(tempCameraObj, renderTexture);

            return thumbnail;
        }

        /// <summary>
        /// 썸네일 카메라 설정
        /// Setup thumbnail camera
        /// </summary>
        /// <param name="tempCameraObj">임시 카메라 오브젝트 / Temporary camera object</param>
        /// <param name="characterObj">캐릭터 오브젝트 / Character object</param>
        /// <returns>설정된 카메라 / Configured camera</returns>
        private Camera SetupThumbnailCamera(GameObject tempCameraObj, GameObject characterObj)
        {
            var tempCamera = tempCameraObj.AddComponent<Camera>();

            // 카메라 기본 설정
            // Basic camera settings
            tempCamera.clearFlags = CameraClearFlags.SolidColor;
            tempCamera.backgroundColor = new Color(0, 0, 0, 0); // 투명 배경 / Transparent background
            tempCamera.orthographic = true;
            tempCamera.orthographicSize = fixedCameraSize;
            tempCamera.nearClipPlane = 0.1f;
            tempCamera.farClipPlane = 1000f;
            tempCamera.cullingMask = characterLayerMask;

            // 카메라 위치 설정
            // Set camera position
            Vector3 cameraPosition = useFixedPosition
                ? characterObj.transform.position + cameraOffset
                : GetCharacterBounds(characterObj).center + cameraOffset;

            tempCamera.transform.position = cameraPosition;

            return tempCamera;
        }

        /// <summary>
        /// 렌더 텍스처 생성
        /// Create render texture
        /// </summary>
        /// <returns>생성된 렌더 텍스처 / Created render texture</returns>
        private RenderTexture CreateRenderTexture()
        {
            var renderTexture = new RenderTexture(thumbnailWidth, thumbnailHeight, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 4; // 안티앨리어싱 / Anti-aliasing
            return renderTexture;
        }

        /// <summary>
        /// 렌더 텍스처를 Texture2D로 변환
        /// Convert render texture to Texture2D
        /// </summary>
        /// <param name="renderTexture">렌더 텍스처 / Render texture</param>
        /// <returns>변환된 텍스처 / Converted texture</returns>
        private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            var thumbnail = new Texture2D(thumbnailWidth, thumbnailHeight, TextureFormat.RGBA32, false);
            thumbnail.ReadPixels(new Rect(0, 0, thumbnailWidth, thumbnailHeight), 0, 0);
            thumbnail.Apply();
            RenderTexture.active = null;

            return thumbnail;
        }

        /// <summary>
        /// 썸네일 생성 관련 리소스 정리
        /// Cleanup thumbnail generation resources
        /// </summary>
        /// <param name="tempCameraObj">임시 카메라 오브젝트 / Temporary camera object</param>
        /// <param name="renderTexture">렌더 텍스처 / Render texture</param>
        private void CleanupThumbnailResources(GameObject tempCameraObj, RenderTexture renderTexture)
        {
            if (Application.isPlaying)
            {
                Destroy(tempCameraObj);
                Destroy(renderTexture);
            }
            else
            {
                DestroyImmediate(tempCameraObj);
                DestroyImmediate(renderTexture);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// 캐릭터의 바운드 계산
        /// Calculate character bounds
        /// </summary>
        /// <param name="characterObj">캐릭터 오브젝트 / Character object</param>
        /// <returns>계산된 바운드 / Calculated bounds</returns>
        private Bounds GetCharacterBounds(GameObject characterObj)
        {
            var renderers = characterObj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return new Bounds(characterObj.transform.position, Vector3.one * 2f);
            }

            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        #endregion

        #region Asset Saving

        /// <summary>
        /// 프리팹 에셋 저장
        /// Save prefab asset
        /// </summary>
        /// <param name="prefabObj">프리팹 오브젝트 / Prefab object</param>
        /// <param name="prefabData">프리팹 데이터 / Prefab data</param>
        /// <param name="thumbnailTexture">썸네일 텍스처 / Thumbnail texture</param>
        private void SavePrefabAsset(GameObject prefabObj, CharacterPrefabData prefabData, Texture2D thumbnailTexture)
        {
            // 프리팹 저장 경로 생성
            // Create prefab save path
            var prefabPath = GeneratePrefabPath();

            // 폴더 존재 확인 및 생성
            // Check and create folder
            EnsureCharacterPrefabsFolder();

            Debug.Log($"Saving prefab with {prefabData.skinParts.Count} skin parts, " +
                      $"{prefabData.slotColors.Count} colors, and {prefabData.colorPickerPositions.Count} picker positions");

            // 프리팹 에셋 저장
            // Save prefab asset
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabObj, prefabPath, out var success);

            if (success && savedPrefab != null)
            {
                Debug.Log($"The character prefab has been created: {prefabPath}");

                // 썸네일 이미지 저장
                // Save thumbnail image
                if (thumbnailTexture != null)
                {
                    SaveThumbnailImage(thumbnailTexture, prefabPath);
                }

                // 에셋 갱신
                // Refresh assets
                EditorUtility.SetDirty(savedPrefab);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // 에디터에서 선택 및 포커스
                // Select and focus in editor
                Selection.activeObject = savedPrefab;
                EditorGUIUtility.PingObject(savedPrefab);
            }
            else
            {
                Debug.LogError("Failed to save prefab!");
            }

            // 임시 프리팹 오브젝트 제거
            // Remove temporary prefab object
            CleanupPrefabObject(prefabObj);
        }

        /// <summary>
        /// 프리팹 저장 경로 생성
        /// Generate prefab save path
        /// </summary>
        /// <returns>생성된 경로 / Generated path</returns>
        private string GeneratePrefabPath()
        {
            return "Assets/CharacterPrefabs/Character_" +
                   System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".prefab";
        }

        /// <summary>
        /// CharacterPrefabs 폴더 존재 확인 및 생성
        /// Ensure CharacterPrefabs folder exists
        /// </summary>
        private void EnsureCharacterPrefabsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/CharacterPrefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "CharacterPrefabs");
            }
        }

        /// <summary>
        /// 임시 프리팹 오브젝트 정리
        /// Cleanup temporary prefab object
        /// </summary>
        /// <param name="prefabObj">프리팹 오브젝트 / Prefab object</param>
        private void CleanupPrefabObject(GameObject prefabObj)
        {
            if (Application.isPlaying)
            {
                Destroy(prefabObj);
            }
            else
            {
                DestroyImmediate(prefabObj);
            }
        }

        /// <summary>
        /// 썸네일 이미지 저장
        /// Save thumbnail image
        /// </summary>
        /// <param name="thumbnail">썸네일 텍스처 / Thumbnail texture</param>
        /// <param name="prefabPath">프리팹 경로 / Prefab path</param>
        private void SaveThumbnailImage(Texture2D thumbnail, string prefabPath)
        {
            try
            {
                // PNG로 인코딩 (투명도 지원)
                // Encode as PNG (with transparency support)
                var pngData = thumbnail.EncodeToPNG();

                // 파일 경로 생성 (프리팹과 같은 이름, 확장자만 .png)
                // Generate file path (same name as prefab, but with .png extension)
                var imagePath = prefabPath.Replace(".prefab", "_Thumbnail.png");

                // 파일 저장 및 Unity 임포트 설정
                // Save file and configure Unity import settings
                File.WriteAllBytes(imagePath, pngData);
                AssetDatabase.ImportAsset(imagePath);

                // 텍스처 임포트 설정
                // Configure texture import settings
                ConfigureThumbnailImportSettings(imagePath);

                Debug.Log($"Thumbnail saved: {imagePath}");

                // 썸네일 텍스처 정리
                // Cleanup thumbnail texture
                CleanupThumbnailTexture(thumbnail);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save thumbnail: {e.Message}");
                CleanupThumbnailTexture(thumbnail);
            }
        }

        /// <summary>
        /// 썸네일 임포트 설정 구성
        /// Configure thumbnail import settings
        /// </summary>
        /// <param name="imagePath">이미지 경로 / Image path</param>
        private void ConfigureThumbnailImportSettings(string imagePath)
        {
            var textureImporter = AssetImporter.GetAtPath(imagePath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                textureImporter.alphaIsTransparency = true;
                textureImporter.SaveAndReimport();
            }
        }

        /// <summary>
        /// 썸네일 텍스처 정리
        /// Cleanup thumbnail texture
        /// </summary>
        /// <param name="thumbnail">썸네일 텍스처 / Thumbnail texture</param>
        private void CleanupThumbnailTexture(Texture2D thumbnail)
        {
            if (thumbnail != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(thumbnail);
                }
                else
                {
                    DestroyImmediate(thumbnail);
                }
            }
        }

        #endregion
    }
#endif
}
