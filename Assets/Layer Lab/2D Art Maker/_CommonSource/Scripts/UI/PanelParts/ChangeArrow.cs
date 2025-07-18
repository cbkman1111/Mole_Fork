using System;
using UnityEngine;

namespace LayerLab.ArtMaker
{
    public class ChangeArrow : MonoBehaviour
    {
        #region Events
       
        public Action OnChangeLeft;
        public Action OnChangeRight;
       
        #endregion

        #region Transform Management
       
        /// <summary>
        /// 화살표를 특정 트랜스폼에 설정
        /// Set arrow to specific transform
        /// </summary>
        /// <param name="t">부모 트랜스폼 / Parent transform</param>
        public void SetTransform(Transform t)
        {
            transform.SetParent(t, false);
            transform.SetAsFirstSibling();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 화살표 숨기기
        /// Hide arrow
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
       
        #endregion

        #region Button Events
       
        /// <summary>
        /// 왼쪽 화살표 클릭 이벤트
        /// Left arrow click event
        /// </summary>
        public void OnClickLeft()
        {
            AudioManager.Instance.PlaySound(SoundList.ButtonArrow);
            OnChangeLeft?.Invoke();
        }

        /// <summary>
        /// 오른쪽 화살표 클릭 이벤트
        /// Right arrow click event
        /// </summary>
        public void OnClickRight()
        {
            AudioManager.Instance.PlaySound(SoundList.ButtonArrow);
            OnChangeRight?.Invoke();
        }
       
        #endregion
    }
}