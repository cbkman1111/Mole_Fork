using UnityEngine;

public class SceneGame : SceneBase
{
    public SceneGame(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        CameraManager.Instance.MainCamera.backgroundColor = Color.red;
        CameraManager.Instance.MainCamera.transform.position = new Vector3(0f, 0f, -1f);

        UIManager.Instance.OpenPopup<PopupSample>("PopupSample");
        return true;
    }
}
