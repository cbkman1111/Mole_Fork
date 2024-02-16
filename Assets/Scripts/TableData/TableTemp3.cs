using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TableTemp3 : System.IDisposable
{
	///  <summary>
	///  [고유키]
	///  </summary>
	public readonly long ID;

	///  <summary>
	///  [이름]
	///  </summary>
	public readonly string NAME;

	///  <summary>
	///  [나이]
	///  </summary>
	public readonly int AGE;

	///  <summary>
	///  [지역]
	///  </summary>
	public readonly string AREA;

private bool disposed = false;
	public TableTemp3()
	{
		NAME = "";
		AGE = 0;
		AREA = "";
	}
	public void Dispose()
	{
		Dispose(true);
		System.GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
			}
			disposed = true;
		}
	}
}
