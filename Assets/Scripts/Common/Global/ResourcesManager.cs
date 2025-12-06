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
        private Dictionary<string, AsyncOperationHandle> _loadedHandles = new Dictionary<string, AsyncOperationHandle>();

        protected override bool Init()
        {
            // 어드레서블 초기화는 여기서 확실하게 하는 것이 좋습니다.
            Addressables.InitializeAsync();

            return true;
        }

        public T LoadInBuild<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public T[] LoadnBuildAllI<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }

        /// <summary>
        /// 어드레서블 에셋 동기 로드 (메모리 누수 방지 적용)
        /// </summary>
        public GameObject LoadBundle(string address)
        {
            // 1. 이미 로드된 적 있는지 확인 (캐싱)
            if (_loadedHandles.TryGetValue(address, out AsyncOperationHandle handle))
            {
                if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result as GameObject;
                }
                // 유효하지 않은 핸들이면 목록에서 제거
                _loadedHandles.Remove(address);
            }

            // 2. 어드레서블 로드 시도
            var op = Addressables.LoadAssetAsync<GameObject>(address);
            GameObject obj = op.WaitForCompletion(); // 동기 대기

            // 3. 성공 시 핸들 저장
            if (op.Status == AsyncOperationStatus.Succeeded && obj != null)
            {
                _loadedHandles.Add(address, op);
                return obj;
            }
            else
            {
                // 실패 시 즉시 해제하여 찌꺼기 방지
                Addressables.Release(op);
                GiantDebug.LogError($"[ResourcesManager] Load Failed: {address}");
                return null;
            }
        }

        /// <summary>
        /// 사용이 끝난 에셋 메모리 해제 (필수!)
        /// </summary>
        public void UnloadBundle(string address)
        {
            if (_loadedHandles.TryGetValue(address, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle); // 실제 메모리 해제
                _loadedHandles.Remove(address); // 목록에서 제거
            }
        }

        // --- InstantiateAsync 관련 조언 ---
        // InstantiateAsync는 '생성'까지 해버리기 때문에 핸들 관리가 더 복잡합니다.
        // 보통은 LoadBundle로 프리팹 원본을 가져온 뒤, 
        // Unity 기본 Instantiate(prefab)을 쓰는 것이 관리 면에서 훨씬 깔끔합니다.

        public T InstantiateSync<T>(string address, Transform parent) where T : Component
        {
            // 1. 프리팹 원본 로드
            GameObject prefab = LoadBundle(address);

            if (prefab != null)
            {
                // 2. 복제(생성)
                GameObject instance = Instantiate(prefab, parent);
                return instance.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// [추가] 위치와 회전값을 지정하여 생성하는 오버로딩 함수
        /// </summary>
        public T InstantiateAsync<T>(string address, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            // 1. 프리팹 원본을 로드합니다. (이때 핸들은 LoadBundle 내부에서 캐싱됩니다)
            GameObject prefab = LoadBundle(address);

            if (prefab != null)
            {
                // 2. 유니티 기본 Instantiate를 사용하여 위치/회전을 지정해 생성합니다.
                // Addressables.InstantiateAsync를 쓰지 않는 이유는, 
                // 생성된 '인스턴스'마다 핸들을 따로 관리하기가 매우 까다롭기 때문입니다.
                // '원본 프리팹'만 관리하는 것이 메모리 관리에 훨씬 유리합니다.
                GameObject instance = Object.Instantiate(prefab, position, rotation, parent);

                // (선택) 인스턴스 이름 깔끔하게 정리
                instance.name = prefab.name;

                return instance.GetComponent<T>();
            }

            return null;
        }
    }
}
