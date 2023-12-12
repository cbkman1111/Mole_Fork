using BehaviorDesigner.Runtime;
using UnityEngine;

public class MazeMouse : MonoBehaviour
{
    public enum Dir
    {
        Up,
        Down,
        Left,
        Right,
    }

    private BehaviorTree behaviorTree = null;
    public Vector2Int Coordinate { get; set; } = Vector2Int.zero;
    public Vector2Int CoordinateNext { get; set; } = Vector2Int.zero;

    public int Direction { get; set; }

    public GameObject[] Arrow;

    private void Awake()
    {
        behaviorTree = transform.GetComponent<BehaviorTree>();
        behaviorTree.SetVariableValue("Mouse", transform);

        Direction = (int)Dir.Down;
        SetArrow();
    }

    public void SetArrow()
    {
        foreach (var arrow in Arrow)
        {
            arrow.gameObject.SetActive(false);
        }

        Arrow[Direction].gameObject.SetActive(true);
    }

    /*
    public Vector2Int GetNext()
    {
        var scene = AppManager.Instance.CurrScene as SceneMaze;
        var maze = scene.mazeGenerator.maze;

        int[] frontY = new int[] { -1, 0, 1, 0 };
        int[] frontX = new int[] { 0, -1, 0, 1 };
        int[] rightY = new int[] { 0, -1, 0, 1 };
        int[] rightX = new int[] { 1, 0, -1, 0 };

        // 1. ���� �ٶ󺸴� ������ �������� ���������� �� �� �ִ��� Ȯ��
        var PosX = Coordinate.x;
        var PosY = Coordinate.y;

        if (maze[PosY + rightY[Direction], PosX + rightX[Direction]] == Board.TileType.Empty)
        {
            // ������ �������� 90�� ȸ��
            _dir = (_dir - 1 + 4) % 4;

            // ������ 1�� ����
            PosY = PosY + frontY[_dir];
            PosX = PosX + frontX[_dir];
            _points.Add(new Pos(PosY, PosX));
        }
        // 2. ���� �ٶ󺸴� ������ �������� ���� �� �� �ִ��� Ȯ��
        else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
        {
            // ������ 1�� ����
            PosY = PosY + frontY[_dir];
            PosX = PosX + frontX[_dir];
            _points.Add(new Pos(PosY, PosX));
        }
        else
        {
            // ���� �������� 90�� ȸ��
            _dir = (_dir + 1 + 4) % 4;
        }

        return Vector2Int.zero;
    }
    */
}
