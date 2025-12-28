using System.Collections.Generic;
using Common.Global.Singleton;
using Common.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.Global
{
    public class ResourcesManager : MonoSingleton<ResourcesManager>
    {
        // 핸들 캐싱
        private Dictionary<string, AsyncOperationHandle> _loadedHandles = new Dictionary<string, AsyncOperationHandle>();
        // [추가] 참조 카운트 관리 (같은 에셋을 여러 곳에서 로드했을 때 조기 해제 방지)
        private Dictionary<string, int> _referenceCounts = new Dictionary<string, int>();

        protected override bool Init()
        {
            Addressables.InitializeAsync();
            return true;
        }

        #region Legacy Resources (In Build)
        public T LoadInBuild<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        // [수정] 메서드 이름 오타 수정
        public T[] LoadAllInBuild<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }
        /// <summary>
        /// Resources 폴더의 프리팹을 로드하고 즉시 생성합니다.
        /// 사용법: InstantiateInBuild<GameObject>("Path/To/Prefab", parentTransform);
        /// </summary>
        public T InstantiateInBuild<T>(string path, Transform parent = null) where T : Object
        {
            T prefab = Resources.Load<T>(path);

            if (prefab != null)
            {
                T instance = Object.Instantiate(prefab, parent);

                // (선택) 생성된 객체 이름에서 "(Clone)" 제거가 필요하면 아래 주석 해제
                // instance.name = prefab.name; 

                return instance;
            }

            Debug.LogError($"[ResourcesManager] Instantiate Failed. Path not found: {path}");
            return null;
        }

        /// <summary>
        /// 위치와 회전값을 지정하여 생성합니다.
        /// 사용법: InstantiateInBuild<Card>("Path/Card", pos, rot, parent);
        /// </summary>
        public T InstantiateInBuild<T>(string path, Vector3 position, Quaternion rotation, Transform parent = null) where T : Object
        {
            T prefab = Resources.Load<T>(path);

            if (prefab != null)
            {
                T instance = Object.Instantiate(prefab, position, rotation, parent);
                return instance;
            }

            Debug.LogError($"[ResourcesManager] Instantiate Failed. Path not found: {path}");
            return null;
        }
        #endregion

        #region Addressables (AssetBundle)

        /// <summary>
        /// 어드레서블 에셋 동기 로드 (참조 카운팅 적용)
        /// GameObject 외에 다른 타입도 로드 가능하도록 제네릭 변경
        /// </summary>
        public T LoadAssetSync<T>(string address) where T : Object
        {
            // 1. 이미 로드된 핸들이 있는지 확인
            if (_loadedHandles.TryGetValue(address, out AsyncOperationHandle handle))
            {
                if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // [중요] 이미 로드되어 있다면 참조 카운트만 증가시키고 리턴
                    _referenceCounts[address]++;
                    return handle.Result as T;
                }

                // 핸들이 유효하지 않다면 정리 후 재로드 시도
                _loadedHandles.Remove(address);
                _referenceCounts.Remove(address);
            }

            // 2. 어드레서블 로드 시도 (동기 대기)
            var op = Addressables.LoadAssetAsync<T>(address);
            T result = op.WaitForCompletion();

            // 3. 성공 시 핸들 저장 및 카운트 초기화
            if (op.Status == AsyncOperationStatus.Succeeded && result != null)
            {
                _loadedHandles.Add(address, op);
                // [중요] 새로 로드했으므로 카운트 1
                if (_referenceCounts.ContainsKey(address))
                    _referenceCounts[address]++;
                else
                    _referenceCounts.Add(address, 1);

                return result;
            }
            else
            {
                Addressables.Release(op);
                Debug.LogError($"[ResourcesManager] Load Failed: {address}");
                return null;
            }
        }

        /// <summary>
        /// GameObject 전용 래퍼 (기존 호환성 유지)
        /// </summary>
        public GameObject LoadBundle(string address)
        {
            return LoadAssetSync<GameObject>(address);
        }

        /// <summary>
        /// 사용이 끝난 에셋 메모리 해제
        /// 참조 카운트가 0이 될 때만 실제 메모리 해제 수행
        /// </summary>
        public void UnloadBundle(string address)
        {
            if (!_loadedHandles.ContainsKey(address)) return;

            // 참조 카운트 감소
            if (_referenceCounts.ContainsKey(address))
            {
                _referenceCounts[address]--;
            }

            // 카운트가 0 이하일 때만 실제 Release 수행
            if (_referenceCounts[address] <= 0)
            {
                if (_loadedHandles.TryGetValue(address, out AsyncOperationHandle handle))
                {
                    Addressables.Release(handle);
                    _loadedHandles.Remove(address);
                    _referenceCounts.Remove(address);

                    // Debug.Log($"[ResourcesManager] Real Unload: {address}");
                }
            }
        }

        #endregion

        #region Instantiate

        public T InstantiateSync<T>(string address, Transform parent = null) where T : Component
        {
            GameObject prefab = LoadAssetSync<GameObject>(address);

            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, parent);
                instance.name = prefab.name; // 이름 깔끔하게

                // GetComponent가 실패할 수 있으므로 TryGetComponent 권장, 
                // 없으면 AddComponent를 하거나 null 리턴 (여기선 기존 로직 유지)
                return instance.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// [수정] 이름 변경 (Async -> Sync), 위치/회전 지정 생성
        /// </summary>
        public T InstantiateSync<T>(string address, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            GameObject prefab = LoadAssetSync<GameObject>(address);

            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab, position, rotation, parent);
                instance.name = prefab.name;
                return instance.GetComponent<T>();
            }

            return null;
        }

        #endregion
    }
}