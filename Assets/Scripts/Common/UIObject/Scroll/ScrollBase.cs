using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UIObject.Scroll
{
	public abstract class ScrollBase<T> : ScrollRect
	{
		private List<int> _displayList = null;
		private List<ScrollCell<T>> _cellList = null;

		protected ScrollPool Pool = null;
		public int CurrIndex = -1; // 디스플레이하고 있는 첫번째 노드 인덱스.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="arry"></param>
		/// <returns></returns>
		public virtual bool Init(RectTransform[] arry)
		{
			if (Pool == null)
			{
				Pool = new ScrollPool();
				Pool.Init(content, 1, arry);
			}

			if (_cellList == null)
			{
				_cellList = new List<ScrollCell<T>>();
			}

			if (_displayList == null)
			{
				_displayList = new List<int>();
			}

			_cellList.ForEach(cell => ReturnObject(cell));
			SetLayoutVertical();
			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public bool SetItems(List<T> list)
		{
			if (list == null)
			{
				return false;
			}

			MEC.Timing.RunCoroutine(CoSetItems(list));
			return true;
		}

		/// <summary>
		/// 리스트를 위로 추가합니다.
		/// </summary>
		/// <param name="list"></param>
		public bool InsertItems(List<T> list)
		{
			if (list == null)
			{
				return false;
			}

			MEC.Timing.RunCoroutine(CoInsertMessage(list));
			return true;
		}

		/// <summary>
		/// 데이터 추가.
		/// </summary>
		/// <param name="data"></param>
		public bool AddItem(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				return false;
			}

			MEC.Timing.RunCoroutine(CoAddMessage(list));
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Update()
		{
			if (enabled == false)
				return;

			if (_cellList == null)
				return;

			CurrIndex = 0;
			if (_displayList.Count > 0)
				CurrIndex = _displayList[0];

			_displayList.Clear();

			for (int i = CurrIndex - 1; i < _cellList.Count; i++)
			{
				if (i < 0 || i >= _cellList.Count)
					continue;

				var node = _cellList[i];
				CheckPosition(node);

				if (node.State == CellState.Out_Up)
				{
					ReturnObject(node);
				}
				else if (node.State == CellState.Out_Down)
				{
					ReturnObject(node);
					break;
				}
				else if (node.State == CellState.InSide)
				{
					if (node.RectTrans == null)
					{
						node.RectTrans = GetObject(node);
						UpdateCell(node.Data, node.RectTrans, i);
					}

					node.Position = new Vector2(0, node.Position.y);
					_displayList.Add(i);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private IEnumerator<float> CoSetItems(List<T> list)
		{
			_cellList.ForEach(cell => ReturnObject(cell));
			_cellList.Clear();
			enabled = false;

			yield return MEC.Timing.WaitForOneFrame;
			yield return MEC.Timing.WaitForOneFrame;

			float totalHeight = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				ScrollCell<T> node = new ScrollCell<T>();
				node.Data = list[i];
				node.Position = Vector2.zero;
				node.RectTrans = GetPreLoaded(node);

				AddCell(node);
				yield return MEC.Timing.WaitForOneFrame;

				float height = node.RectTrans.rect.height;
				node.Height = height;
				node.RectTrans = null;
			}

			yield return MEC.Timing.WaitForOneFrame;
			for (int i = 0; i < _cellList.Count; i++)
			{
				var node = _cellList[i];
				var frontNode = GetFront(i);
				if (frontNode == null)
				{
					node.Position = new Vector2(0, 0);
				}
				else
				{
					node.Position = new Vector2(0, frontNode.Position.y - frontNode.Height);
				}

				totalHeight += node.Height;
			}

			content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
			verticalNormalizedPosition = 0f;
			enabled = true;
			yield return MEC.Timing.WaitForOneFrame;
		}


		/// <summary>
		/// InsertMessage 코루틴 처리.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private IEnumerator<float> CoInsertMessage(List<T> list)
		{
			yield return MEC.Timing.WaitForOneFrame;

			enabled = false;
			ScrollCell<T> firstNode = _cellList[0];
			float AddHeight = 0;
			for (int i = 0; i < list.Count; i++)
			{
				ScrollCell<T> node = new ScrollCell<T>();
				node.Data = list[i];
				node.RectTrans = GetPreLoaded(node); 
				Insert(node);

				yield return MEC.Timing.WaitForOneFrame;
				float height = node.RectTrans.rect.height;
				node.Height = height;
				node.RectTrans = null;
				AddHeight += height;
			}

			yield return MEC.Timing.WaitForOneFrame;
			for (int i = 0; i < _cellList.Count; i++)
			{
				var node = _cellList[i];
				var frontNode = GetFront(i);
				if (frontNode == null)
				{
					node.Position = new Vector2(0, 0);
				}
				else
				{
					node.Position = new Vector2(0, frontNode.Position.y - frontNode.Height);
				}
			}

			content.localPosition += new Vector3(0, AddHeight, 0);
			content.sizeDelta += new Vector2(0, AddHeight);
			enabled = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private IEnumerator<float> CoAddMessage(List<T> list)
		{
			yield return MEC.Timing.WaitForOneFrame;

			enabled = false;

			int last = _cellList.Count - 1;
			ScrollCell<T> lastNode = _cellList[last];
			bool bottom = false;
			if (lastNode.RectTrans != null)
			{
				bottom = true;
			}

			for (int i = 0; i < list.Count; i++)
			{
				ScrollCell<T> node = new ScrollCell<T>();
				node.Data = list[i];
				node.Position = Vector2.zero;
				node.RectTrans = GetPreLoaded(node);
				AddCell(node);
				yield return MEC.Timing.WaitForOneFrame;

				float height = node.RectTrans.rect.height;
				node.Height = height;
				node.RectTrans = null;
				node.Position = new Vector2(0, lastNode.Position.y - lastNode.Height);

				lastNode = node;
				content.sizeDelta += new Vector2(0, height);
			}

			yield return MEC.Timing.WaitForOneFrame;

			if (bottom == true)
			{

				DOTween.To(() => verticalNormalizedPosition, x => verticalNormalizedPosition = x, 0, 0.2f);//.SetEase(ease:);
				//ScrollSmoothToPointVertical(
				//verticalNormalizedPosition = 0;
			}

			enabled = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private bool AddCell(ScrollCell<T> cell)
		{
			UpdateCell(cell.Data, cell.RectTrans);
			_cellList.Add(cell);
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private bool Insert(ScrollCell<T> cell)
		{
			UpdateCell(cell.Data, cell.RectTrans);
			_cellList.Insert(0, cell);
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private void CheckPosition(ScrollCell<T> cell)
		{
			Vector2 contentPosition = content.localPosition;
			Vector2 top = contentPosition + cell.Position;// + cell.ScrollPosition;
			Vector2 bottom = new Vector2(
				top.x,
				top.y - cell.Height);

			if (viewport.rect.min.y <= top.y && viewport.rect.max.y >= bottom.y)
			{
				cell.State = CellState.InSide;
			}
			else
			{
				if (viewRect.rect.min.y > top.y)
					cell.State = CellState.Out_Down;
				else
					cell.State = CellState.Out_Up;
			}
		}

		/// <summary>
		/// curr 앞의 셀 정보 리턴.
		/// </summary>
		/// <param name="curr"></param>
		/// <returns></returns>
		private ScrollCell<T> GetFront(int curr)
		{
			int front = curr - 1;
			if (front < 0 || front >= _cellList.Count)
			{
				return null;
			}

			return _cellList[front];
		}

		/// <summary>
		/// Cell 을 업데이트.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="rectTrans"></param>
		protected abstract void UpdateCell(T data, RectTransform trans, int index = 0);
		/// <summary>
		/// 크기 계산을 위해 프리로드한 셀에 해당하는 노드를 리턴.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		protected abstract RectTransform GetPreLoaded(ScrollCell<T> cell);
		/// <summary>
		/// 오브젝트 풀에서 사용 가능한 객체 리턴.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		protected abstract RectTransform GetObject(ScrollCell<T> cell);
		/// <summary>
		/// 사용이 만료된 객체를 풀에 반환.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		protected virtual bool ReturnObject(ScrollCell<T> cell)
		{
			if (cell == null)
				return false;

			bool success = Pool.ReturnObject(cell.RectTrans);
			if (success == true)
			{
				cell.RectTrans = null;
			}

			return success;
		}
		/// <summary>
		/// 스크롤뷰 드래그 이벤트 전달.
		/// </summary>
		protected abstract void OnDragScroll();

		public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
		{
			base.OnDrag(eventData); //호출하면 컨텐츠 뷰가 스크롤되니까 호출 안함.

			OnDragScroll();
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
}
