using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollBase<T> : ScrollRect
{
    private Pool<RectTransform> pool = null;
    protected RectTransform prefab = null; // 스크롤 아이템.
    protected List<Cell> CellList = null; // 전체 데이터.

    public int CurrIndex = -1;

    public enum State
    {
        InSide = 0,
        Out_Up,
        Out_Down,
    };

    /// <summary>
    /// 
    /// </summary>
    public class Cell
    {
        public int Index;
        public T Data;

        public State State;
        private RectTransform trans;
        private float height;
        public RectTransform RectTrans
        {
            get
            {
                return trans;
            }
            set
            {
                trans = value;
            }
        }

        /// <summary>
        /// 높이.
        /// </summary>
        /// <returns></returns>

        public float Height {
            set 
            {
                height = value;
            }
            get 
            {
                return height;
            } 
        }
        

        /// <summary>
        /// 
        /// </summary>
        Vector2 scrollPosition = Vector2.zero;
        public Vector2 ScrollPosition {
            get
            {
                return scrollPosition;
            }
            set
            {
                scrollPosition = value;
                if (RectTrans != null)
                    RectTrans.localPosition = oriPosition + scrollPosition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        Vector2 oriPosition = Vector2.zero;
        public Vector2 Position {
            get
            {
                return oriPosition;
            }
            set
            {
                oriPosition = value;
                if (RectTrans != null)
                    RectTrans.localPosition = oriPosition;// + scrollPosition;
            }
        }
    }

    // 노드 정보.
    public virtual bool Init(RectTransform prefab)
    {
        this.prefab = prefab;
        this.CellList = new List<Cell>();
        this.pool = Pool<RectTransform>.Create(prefab, content.transform, 20);
   
        SetLayoutVertical();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    protected bool Add(List<T> list)
    {
        if (list == null)
        {
            return false;
        }

        List<Cell> listAdd = new List<Cell>();
        var cell = pool.GetObject(); // 하나 꺼내서 사이즈 계산에 사용.
        Vector2 size = Vector2.zero;
        for (int i = 0; i < list.Count; i++)
        {
            Cell node = new Cell();
            node.Index = i;
            node.Data = list[i];
            node.Position = Vector2.zero;
            node.RectTrans = cell;
            if (node.RectTrans != null)
            {
                CellUpdate(node);
                size.y += node.Height;
            }

            var frontNode = GetFront(node.Index);
            if (frontNode == null)
            {
                node.Position = new Vector2(0, 0);
            }
            else
            {
                node.Position = new Vector2(0, frontNode.Position.y - frontNode.Height);
            }

            node.RectTrans = null;
            listAdd.Add(node);
        }

        pool.ReturnObject(cell); // 계산 끝난건 다시 되돌림.
        //content.sizeDelta += size;
        //verticalNormalizedPosition = 0f;

        for (int i = 0; i < listAdd.Count; i++)
        {
            var node = listAdd[i];
            if (node == null)
                continue;

            CheckPosition(node, out State state);
            if (state == State.Out_Down || state == State.Out_Up)
            {
                pool.ReturnObject(node.RectTrans);
                node.RectTrans = null;
            }
            else
            {
                if (CurrIndex == -1 || CurrIndex > node.Index)
                    CurrIndex = node.Index;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    protected bool Set(List<T> list)
    {
        CellList.Clear();

        if (list == null)
        {
            return false;
        }

        var cell = pool.GetObject(); // 하나 꺼내서 사이즈 계산에 사용.
        Vector2 size = Vector2.zero;
        for (int i = 0; i < list.Count; i++)
        {
            Cell node = new Cell();
            node.Index = i;
            node.Data = list[i];
            node.Position = Vector2.zero;
            node.RectTrans = cell;
            if (node.RectTrans != null)
            {
                CellUpdate(node);
                size.y += node.Height;
            }
            
            var frontNode = GetFront(node.Index);
            if (frontNode == null)
            {
                node.Position = new Vector2(0, 0);
            }
            else
            {
                node.Position = new Vector2(0, frontNode.Position.y - frontNode.Height);
            }

            node.RectTrans = null;
            CellList.Add(node);
        }

        pool.ReturnObject(cell); // 계산 끝난건 다시 되돌림.
        content.sizeDelta = size;
        verticalNormalizedPosition = 0f;

        for (int i = 0; i < CellList.Count; i++)
        {
            var node = CellList[i];
            if (node == null)
                continue;

            CheckPosition(node, out State state);
            if (state == State.Out_Down || state == State.Out_Up)
            {
                pool.ReturnObject(node.RectTrans);
                node.RectTrans = null;
            }
            else
            {
                if(CurrIndex == -1 || CurrIndex > i)
                    CurrIndex = i;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    private void CellUpdate(Cell cell)
    {
        UpdateCell(cell.Data, cell.RectTrans);
        Canvas.ForceUpdateCanvases();

        cell.Height = cell.RectTrans.rect.height;
    }

    /// <summary>
    /// 데이터 추가.
    /// </summary>
    /// <param name="data"></param>
    public void InsertMessage(T data)
    {
        //int last = CellList.Count - 1;
        //Cell lastNode = CellList[last];
        Cell firstNode = CellList[0];
        float AddHeight = 0;

        var obj = pool.GetObject();
        for (int i = 0; i < 10; i++)
        {
            Cell node = new Cell();
            node.Index = firstNode.Index + 1;
            node.Data = data;
            node.Position = Vector2.zero;
            node.RectTrans = obj;
            if (node.RectTrans != null)
            {
                CellUpdate(node);
                AddHeight += node.Height;
            }

            node.Position = new Vector2(0, firstNode.Position.y);
            CellList.Insert(0, node);
        }

        content.localPosition += new Vector3(0, AddHeight, 0);
        content.sizeDelta += new Vector2(0, AddHeight);
        pool.ReturnObject(obj);

        for (int i = 1; i < CellList.Count; i++)
        {
            var cell = CellList[i];
            var frontNode = GetFront(i);

            cell.Position = new Vector2(0, frontNode.Position.y - frontNode.Height);
        }
    }

    /// <summary>
    /// 상태 처리.
    /// </summary>
    private void Update()
    {  
        if (CellList == null)
            return;

        int nextIndex = -1;
        for (int i = CurrIndex - 1; i < CurrIndex + pool.Max; i++)
        {
            if (i < 0 || i >= CellList.Count)
                continue;

            var cell = CellList[i];
            CheckPosition(cell, out State state);
            if (state == State.Out_Down || state == State.Out_Up)
            {
                if (cell.RectTrans != null)
                {
                    pool.ReturnObject(cell.RectTrans);
                    cell.RectTrans = null;
                }
            }
            else if (state == State.InSide)
            {
                if(nextIndex == -1 || nextIndex > i)
                    nextIndex = i;

                if (cell.RectTrans == null)
                {
                    var newCell = pool.GetObject();
                    if (newCell != null)
                    {
                        cell.RectTrans = newCell;
                        CellUpdate(cell);
                        Canvas.ForceUpdateCanvases();
                        cell.Position = new Vector2(0, cell.Position.y);
                    }
                }
            }
        }

        CurrIndex = nextIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    void CheckPosition(Cell cell, out State direction)
    {
        Vector2 contentPosition = content.localPosition;
        Vector2 top = contentPosition + cell.Position;// + cell.ScrollPosition;
        Vector2 bottom = new Vector2(
            top.x,
            top.y - cell.Height);
        
        if (viewport.rect.min.y <= top.y && viewport.rect.max.y >= bottom.y)
        {
            direction = State.InSide;
        }
        else
        {
            if (viewRect.rect.min.y > top.y)
                direction = State.Out_Down;
            else
                direction = State.Out_Up;
        }
    }

    protected Cell GetFront(int curr)
    {
        int front = curr - 1;
        if (front < 0 || front >= CellList.Count)
        {
            return null;
        }

        return CellList[front];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rectTrans"></param>
    protected abstract void UpdateCell(T data, RectTransform rectTrans);

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnDrag(eventData); //호출하면 컨텐츠 뷰가 스크롤되니까 호출 안함.
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void OnScroll(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnScroll(data);
    }
}
