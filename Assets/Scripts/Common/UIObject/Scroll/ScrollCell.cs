using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InfinityScroll
{
	public enum CellState
	{
		InSide = 0,
		Out_Up,
		Out_Down,
	};

	/// <summary>
	/// 
	/// </summary>
	public class ScrollCell<T>
	{
		public T Data;
		public CellState State;
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

		public float Height
		{
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
		Vector2 oriPosition = Vector2.zero;
		public Vector2 Position
		{
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
}
