using Common.UIObject.Scroll;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 채팅 스크롤에 사용되는 프리팹 타입.
/// </summary>
public enum TestPrefabType
{
	Test_Type_A = 0,
	Test_Type_B,
	Max
}

public class ScrollTest : ScrollBase<ScrollData>
{
	private RectTransform[] preLoaded = null;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="arry"></param>
	/// <returns></returns>
	public override bool Init(RectTransform[] arry)
	{
		if (base.Init(arry) == false)
			return false;

		if (preLoaded == null)
		{
			preLoaded = new RectTransform[(int)TestPrefabType.Max];
			for (int i = 0; i < arry.Length; i++)
			{
				preLoaded[i] = arry[i];
				preLoaded[i].localPosition = new Vector3(0, 1000, 0);
			}
		}

		verticalNormalizedPosition = 0;
		return true;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private RectTransform GetObject(TestPrefabType type)
	{
		string name = preLoaded[(int)type].name;
		return Pool.GetObject(name);
	}

	protected override RectTransform GetObject(ScrollCell<ScrollData> cell)
    {
		if (cell == null)
			return null;

		return GetObject(cell.Data.PrefabType());
	}

    protected override RectTransform GetPreLoaded(ScrollCell<ScrollData> cell)
    {
		return preLoaded[(int)cell.Data.PrefabType()];
	}

    protected override void OnDragScroll(){}

    protected override void UpdateCell(ScrollData data, RectTransform trans, int index = 0)
    {
		if (trans == null)
			return;

		var cell = trans.GetComponent<UICellTest>();
		if (cell != null)
		{
			cell.SetUI(data);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
	}
}

public class ScrollData
{
	public int id;
	public TestPrefabType type;
	public string msg;
}

public static class ScrollDataExt
{
	public static TestPrefabType PrefabType(this ScrollData data)
	{
		return data.type;
	}

	public static string GetName(this ScrollData data)
	{
		return data.msg;
	}
}