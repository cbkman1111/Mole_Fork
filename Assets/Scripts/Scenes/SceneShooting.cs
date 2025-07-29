using Common.Global;
using Common.Scene;
using Creature;
using UI.Menu;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneShooting : SceneBase
{
    public Human Player = null;
    private UIMenuShooting Menu = null;
    public override bool Init(JSONObject param)
    {
        Menu = UIManager.Instance.OpenMenu<UIMenuShooting>();
        Menu.InitMenu();
        Menu.Joystick.OnMove = OnMove;
        Menu.Joystick.OnStop = OnStop;
        return true;
    }

    public void OnMove(Vector3 angle, float f)
    {
        // 탑뷰 시점으로 변환.
        angle.z = angle.y;
        IMove moveAble = Player as IMove;
        if (moveAble != null)
        {
            moveAble.Move(angle);
        }
    }

    public void OnDash(Vector3 angle)
    {
        // 탑뷰 시점으로 변환.
        angle.z = angle.y;
        IMove moveAble = Player as IMove;
        if (moveAble != null)
        {
            moveAble.Dash(angle);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnStop()
    {
        IMove moveAble = Player as IMove;
        if (moveAble != null)
        {
            moveAble.Stop();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public override void OnTouchBean(Vector3 position)
    {
        if (Menu == null || Menu.Joystick == null)
            return;

        Menu.Joystick.TouchBegin(position);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public override void OnTouchEnd(Vector3 position)
    {
        if (Menu == null || Menu.Joystick == null)
            return;

        Menu.Joystick.TouchEnd(position);

        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
    {
        if (Menu == null || Menu.Joystick == null)
            return;

        Menu.Joystick.TouchMove(position);
    }
}
