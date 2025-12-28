using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gostop
{
    public class Card : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private SpriteRenderer spriteRendererDebug = null; // 디버그용(주인 표시)
        [SerializeField] private MeshRenderer meshRenderer = null; // 그림자용
        [SerializeField] private BoxCollider boxCollider = null;
        //[SerializeField] private Rigidbody rigidBody = null;

        // [추가] 외부에서 필요하다면 프로퍼티로 접근 (캡슐화)
        //public Rigidbody RigidBody => rigidBody;

        // --- 데이터 프로퍼티 ---
        public int Num { get; private set; }
        public int Month { get; private set; }
        public KindOf KindOfCard { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        private Board.Player owner = Board.Player.None;
        public Board.Player Owner
        {
            get => owner;
            set
            {
                owner = value;
                UpdateDebugColor();
            }
        }

        /// <summary>
        /// 이 카드가 피(껍질) 종류인지 여부
        /// </summary>
        public bool IsPe => KindOfCard == KindOf.P || KindOfCard == KindOf.PP || KindOfCard == KindOf.PPP;

        /// <summary>
        /// 피의 가치 (일반피=1, 쌍피=2, 쓰리피=3)
        /// </summary>
        public int PeCount => KindOfCard switch
        {
            KindOf.P => 1,
            KindOf.PP => 2,
            KindOf.PPP => 3,
            _ => 0 // 그 외(광, 열끗 등)는 0 리턴
        };

        public enum KindOf
        {
            GWANG,
            GWANG_B,

            HONG,
            CHUNG,
            CHO,
            CHO_B,

            MUNG,
            MUNG_GODORI,
            MUNG_KOO,

            P,
            PP,
            PPP,
        }

        private void Awake()
        {
            // 컴포넌트 자동 캐싱 (실수 방지)
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            //if (rigidBody == null) rigidBody = GetComponent<Rigidbody>();
        }

        private void OnDisable()
        {
            // [중요] 오브젝트가 꺼질 때 이 트랜스폼에 걸린 모든 트윈을 즉시 종료
            // LateUpdate에서 관리할 필요 없이 이게 가장 깔끔하고 확실함
            transform.DOKill();
        }

        public bool Init(int num, Sprite sprite)
        {
            Num = num;
            Month = GetMonth(num);
            Owner = Board.Player.None;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = 1;
            }

            Height = boxCollider.size.y;
            Width = boxCollider.size.x;

            SetCardType(Num);
            SetOpen(false, null);
            SetEnablePhysics(false);

            gameObject.name = $"{Month}M_{Num}_{KindOfCard}"; // 디버깅 편하게 이름 변경
            return true;
        }
        public bool ReplaceCard(int num, Sprite sprite)
        {
            Num = num;
            Month = GetMonth(num);

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = 1;
            }

            Height = boxCollider.size.y;
            Width = boxCollider.size.x;
            gameObject.name = $"{Month}M_{Num}_{KindOfCard}"; // 디버깅 편하게 이름 변경

            SetCardType(Num);
            return true;
        }
        public Sprite GetSprite()
        {
            return spriteRenderer.sprite;
        }

        public void SetSortOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        private void UpdateDebugColor()
        {
            if (spriteRendererDebug == null) return;

            spriteRendererDebug.color = owner switch
            {
                Board.Player.Me => Color.blue,
                Board.Player.Enemy => Color.red,
                _ => Color.white
            };
        }

        /// <summary>
        /// 번호에 따른 카드 타입 분류 (Init 내부 정리)
        /// </summary>
        private void SetCardType(int num)
        {
            switch (Num)
            {
                case 1:
                case 9:
                case 29:
                case 41:
                    KindOfCard = KindOf.GWANG;
                    break;
                case 45:
                    KindOfCard = KindOf.GWANG_B;
                    break;
                case 5:
                case 13:
                case 30:
                    KindOfCard = KindOf.MUNG_GODORI;
                    break;
                case 33:
                    KindOfCard = KindOf.MUNG_KOO;
                    break;
                case 17:
                case 21:
                case 25:
                case 37:
                case 46:
                    KindOfCard = KindOf.MUNG;
                    break;
                case 2:
                case 6:
                case 10:
                    KindOfCard = KindOf.HONG;
                    break;
                case 14:
                case 18:
                case 26:
                    KindOfCard = KindOf.CHO;
                    break;
                case 22:
                case 34:
                case 38:
                    KindOfCard = KindOf.CHUNG;
                    break;
                case 47:
                    KindOfCard = KindOf.CHO_B;
                    break;
                case 49:
                    KindOfCard = KindOf.PPP;
                    break;
                case 12:
                case 50:
                case 51:
                case 52:
                    KindOfCard = KindOf.PP;
                    break;
                default:
                    KindOfCard = KindOf.P;
                    break;
            }
        }

        public int GetMonth(int num)
        {
            return (int)Mathf.Floor((num - 1) / 4 + 1);
        }



        public void SetEnablePhysics(bool enable)
        {
            //if (rigidBody != null)
            //    rigidBody.isKinematic = !enable;
        }

        public void MoveTo(Vector3 position, Vector3 scale, Ease ease = Ease.Linear, float time = 0.5f, float delay = 0f, Action complete = null)
        {
            IsAnimating = true;
            transform.DOKill();

            var sequence = DOTween.Sequence();
            sequence.Join(transform.DOMove(position, time));
            sequence.Join(transform.DOScale(scale, time));

            // 3. 공통 설정 적용 (딜레이, 이징, 콜백)
            sequence.SetDelay(delay)
               .SetEase(ease)
               .OnComplete(() => {

                   if (complete != null)
                       complete();
 
               });
        }

        public void JumpTo(Vector3 position, Vector3 scale, float jumpPower = 2.0f, Ease ease = Ease.OutQuad, float time = 0.5f, float delay = 0f, Action complete = null)
        {
            IsAnimating = true;
            transform.DOKill();

            var sequence = DOTween.Sequence();
            sequence.Join(transform.DOJump(position, jumpPower, 1, time));
            sequence.Join(transform.DOScale(scale, time));
            sequence.SetDelay(delay)
                .SetEase(ease)
                .OnComplete(() => {

                    if (complete != null)
                        complete();
  
                });
        }

        public void SetOpen(bool open, Action complete)
        {
            RotateCard(new Vector3(0, 0, open == true ? 180 : 0), 0, 0, complete);
        }

        public void CardOpen(float time = 0.1f, float delay = 0.0f, Action complete = null)
        {
            RotateCard(new Vector3(0, 0, 180), time, delay, complete);
        }

        public void ShowMe(float time = 0.1f, float delay = 0.0f, Action complete = null)
        {
            RotateCard(new Vector3(0, 0, 180), time, delay, complete);
        }

        private void RotateCard(Vector3 targetAngle, float time, float delay, Action complete)
        {
            // [중요] 돌기 전에 물리 끄기 (안 그러면 바닥이랑 충돌해서 떼굴떼굴 구름)
            //SetEnablePhysics(false);

            IsAnimating = true;
            var child = transform.GetChild(0);
            child.DORotate(targetAngle, time)
                .SetDelay(delay)
                .SetEase(Ease.OutBack) // 뒤집을 때 살짝 튕김 (타격감)
                .OnComplete(() => {
                    // 다 돌고 나면 물리 다시 켜기 (필요한 경우만)
                    SetEnablePhysics(true); 
                    
                    if (complete != null)
                        complete();
 
                });
        }

        /*
        public void SetShadow(bool active)
        {
            if (active == true)
            {
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            else
            {
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        */

        /// <summary>
        /// 현재 DOTween으로 움직이거나 회전 중인지 여부
        /// </summary>
        public bool IsAnimating { get; set; } = false;
    }
}
