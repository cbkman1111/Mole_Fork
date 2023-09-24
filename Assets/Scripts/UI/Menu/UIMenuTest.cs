using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuTest : MenuBase
    {
        /*
    public GameObject from;
    public GameObject to;
    public GameObject result;
    public Text angle;

    public int x;
    public int y;
    */

        public override void OnInit()
        {

        }

        public bool InitMenu()
        {
            return true;
        }

        /*
    private void Update()
    {
        float radius = 200;
        float a = CalculateAngle(from.transform, to.transform.position, from.transform.position);

        float x = (Mathf.Sin(a * (Mathf.PI / 180)) * radius);
        float y = (Mathf.Cos(a * (Mathf.PI / 180)) * radius);

        Vector3 dest = from.transform.position;
        dest.x += x;
        dest.y += y;
        result.transform.position = dest;

        SetText(angle, $"{a}");
    }
    */
        public static float CalculateAngle(Transform trans, Vector3 from, Vector3 to)
        {
            //Vector3 v = to - from;
            //float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;    // return : -180 ~ 180 degree (for unity)
            float angle = Vector3.SignedAngle(-trans.up, to - from, trans.forward);
            //float angle = Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
            //float angle = Quaternion.Angle(transform.rotation, target.rotation);
            //float angle = Vector3.Angle(to, from);


            return angle;
        }

        public override void OnValueChanged(InputField input, string str) 
        {
            /*
        if (input.name.CompareTo("InputField - x") == 0)
        {
            x = int.Parse(str);
        }
        else if (input.name.CompareTo("InputField - y") == 0)
        {
            y = int.Parse(str);
        }*/
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
