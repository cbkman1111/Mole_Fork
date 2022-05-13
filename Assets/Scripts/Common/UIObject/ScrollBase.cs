using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ScrollBase<T> : ScrollRect
{
    protected RectTransform cellPrefab = null;

    private List<GameObject> cellList = null; // 보유한 셀.
    private List<int> indexList = null; // 셀의 인덱스.
    protected List<T> dataList = null; // 전체 데이터.
    
    private bool limitTop = false;
    private bool limitBottom = false;
    public float paddingY = 2;
    Vector2 velocity = Vector2.zero;

    protected override void Awake()
    {
        cellList = new List<GameObject>();
        indexList = new List<int>();
        dataList = new List<T>();

        SetLayoutVertical();
    }

    protected abstract void UpdateCell(RectTransform transform, int index);

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData){}

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        var deltaY = eventData.delta.y;
        RectTransform contentTransform = content.GetComponent<RectTransform>();

        var indexLast = contentTransform.childCount - 1;
        var last = contentTransform.GetChild(indexLast).GetComponent<RectTransform>();
        var first = contentTransform.GetChild(0).GetComponent<RectTransform>();
        var size = last.rect.size;

        Vector3 move = Vector3.zero;
        move.y = deltaY;

        bool upSide = (deltaY > 0) ? true : false;


        // 객체들을 옮김.
        foreach (var cell in cellList)
        {
            var rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.localPosition += move;
        }
     
        // 
        if(upSide == true)
        {
            // 최 상단 노드가 탈출.
            Vector2 bottom = new Vector2(
                first.localPosition.x,
                first.localPosition.y - size.y * first.pivot.y);

            Vector2 top = new Vector2(
                first.localPosition.x,
                first.localPosition.y + size.y * first.pivot.y);

            if (contentTransform.rect.Contains(top) == false &&
                contentTransform.rect.Contains(bottom) == false)
            {
                int newIndex = indexList.Last() + 1;
                if (newIndex < dataList.Count)
                {
                    indexList[0] = newIndex;
                    indexList.Remove(newIndex);
                    indexList.Add(newIndex);
                    UpdateCell(first, newIndex);

                    var localposition = first.localPosition;
                    localposition.y = last.localPosition.y - (paddingY + size.y);
                    first.localPosition = localposition;
                    first.SetAsLastSibling();
                }
                else
                {
                    // 밑바닥임.
                    limitBottom = true;
                }
            }
        }
        else
        {   
            // 최 상단 노드가  아래로 탈출.
            Vector2 bottom = new Vector2(
                first.localPosition.x,
                first.localPosition.y - size.y * first.pivot.y);

            Vector2 top = new Vector2(
                first.localPosition.x,
                first.localPosition.y + size.y * first.pivot.y);

            if (contentTransform.rect.Contains(top) == false &&
                contentTransform.rect.Contains(bottom) == false)
            {
                limitTop = true;
                return;
            }
          
            // 최 하단 노드가 화면 탈출.
            bottom = new Vector2(
            last.localPosition.x,
            last.localPosition.y - size.y * last.pivot.y);

            top = new Vector2(
            last.localPosition.x,
            last.localPosition.y + size.y * last.pivot.y);

            if (contentTransform.rect.Contains(top) == false &&
                contentTransform.rect.Contains(bottom) == false)
            {
                int newIndex = indexList.First() - 1;
                if (newIndex >= 0)
                {
                    indexList[indexList.Count - 1] = newIndex;
                    indexList.Remove(newIndex);
                    indexList.Insert(0, newIndex);
                    UpdateCell(last, newIndex);

                    var localposition = last.localPosition;
                    localposition.y = first.localPosition.y + (paddingY + size.y);

                    last.localPosition = localposition;
                    last.SetAsFirstSibling();
                }
                else
                {
                    // 꼭대기임.
                    limitTop = true;
                }
            }
        }

        //Debug.LogWarning($"limited 1 = {limited}");
        //Debug.LogWarning($"limited 2 = {limited}");
        //Debug.Log($"OnDrag {eventData.delta.y}");
    }

    private void Update()
    {
        
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag limitTop = {limitTop}, limitBottom = {limitBottom}");
        if(limitBottom == true || limitTop == true)
        {
            if(limitTop == true)
            {
                RectTransform r = content.GetChild(0).GetComponent<RectTransform>();
                float positionY = -(r.pivot.y * r.rect.size.y);

                Vector3 move = Vector3.zero;
                move.y = positionY - r.localPosition.y;
                foreach (var cell in cellList)
                {
                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.localPosition += move;
                }
            }
            else if(limitBottom == true)
            {
                RectTransform r = content.GetChild(content.childCount-1).GetComponent<RectTransform>();
                float contentHeight = content.rect.height;
                float positionY = -(contentHeight - r.pivot.y * r.rect.size.y);

                Vector3 move = Vector3.zero;
                move.y = positionY - r.localPosition.y;
                foreach (var cell in cellList)
                {
                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.localPosition += move;
                }
            }

            limitTop = false;
            limitBottom = false;
        }
    }

    protected bool Set(List<T> list)
    {
        if(list == null)
        {
            return false;
        }

        cellList.Clear();
        dataList.Clear();

        dataList.AddRange(list);

        float totalHeight = cellPrefab.rect.size.y * list.Count;
        float contentHeight = content.rect.height;

        int count = (int)(contentHeight / cellPrefab.sizeDelta.y) + 1;

        for (int i = 0; i < count; i++)
        {
            GameObject cell = Instantiate(cellPrefab.gameObject, content.transform);

            var rectTransform = cell.GetComponent<RectTransform>();
            Vector2 cellSize = rectTransform.rect.size;
            float positionY = (cellSize.y + paddingY) * -i - rectTransform.pivot.y * cellSize.y;
            float positionX = rectTransform.pivot.x * cellSize.x;
            rectTransform.localPosition = new Vector2(
                positionX,
                positionY);

            cell.name = $"{cell.name}_{i}";

            UpdateCell(rectTransform, i);

            indexList.Add(i);
            cellList.Add(cell);
        }

        normalizedPosition = new Vector2(0, 0);
        return true;
    }
}
