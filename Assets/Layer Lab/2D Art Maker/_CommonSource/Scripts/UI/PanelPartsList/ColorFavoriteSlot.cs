using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LayerLab.ArtMaker
{
    public class ColorFavoriteSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI Components")]
        [SerializeField] private Image colorImage;
        [SerializeField] private GameObject emptyIndicator;
        [SerializeField] private GameObject selectionFrame; // 선택 프레임
        
        private int slotIndex;
        private ColorFavoriteManager favoriteManager;
        private bool isEmpty = true;
        private bool isSelected = false;
        private Color slotColor = Color.clear;
        
        
        /// <summary>
        /// 슬롯 초기화
        /// Initialize slot
        /// </summary>
        /// <param name="index">슬롯 인덱스</param>
        /// <param name="manager">즐겨찾기 매니저</param>
        public void Init(int index, ColorFavoriteManager manager)
        {
            slotIndex = index;
            favoriteManager = manager;
            SetColor(Color.clear);
            SetSelected(false);
        }
        
        /// <summary>
        /// 슬롯 색상 설정
        /// Set slot color
        /// </summary>
        /// <param name="color">설정할 색상</param>
        public void SetColor(Color color)
        {
            slotColor = color;
            isEmpty = (color == Color.clear || color.a == 0);
            
           
            colorImage.gameObject.SetActive(!isEmpty);
                emptyIndicator.SetActive(isEmpty);
            
            if (!isEmpty)
            {
                colorImage.color = color;
            }
        }
        
        /// <summary>
        /// 선택 상태 설정
        /// Set selection state
        /// </summary>
        /// <param name="selected">선택 여부</param>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selectionFrame != null)
            {
                selectionFrame.SetActive(selected);
            }
        }
    
        
        /// <summary>
        /// 클릭 이벤트 처리
        /// Handle click event
        /// </summary>
        /// <param name="eventData">이벤트 데이터</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (favoriteManager == null) return;
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (isEmpty)
                {
                    // 빈 슬롯을 클릭하면 선택
                    favoriteManager.SelectSlot(slotIndex);
                }
                else
                {
                    // 색상이 있는 슬롯을 클릭하면 색상 적용 또는 선택
                    if (isSelected)
                    {
                        // 이미 선택된 슬롯이면 색상 적용
                        favoriteManager.ApplyFavoriteColor(slotIndex);
                    }
                    else
                    {
                        // 선택되지 않은 슬롯이면 선택
                        favoriteManager.SelectSlot(slotIndex);
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // 우클릭으로 선택 및 삭제 (색상이 있을 때만)
                if (!isEmpty)
                {
                    favoriteManager.SelectSlot(slotIndex);
                    favoriteManager.RemoveFavoriteColor(slotIndex);
                }
            }
        }
        
    }
}