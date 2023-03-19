using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnitBase;

public class SceneGame : SceneBase
{
    private Skell.Player player = null;
    private List<UnitBase> monsters = null;
    private Boat boat = null;
    private Magnatic magnatic = null;
    private UIMenuGame menu = null;

    private Boat prefabBoat = null;
    private Magnatic prefabMagnatic = null;
    private Skell.Player prefabPlayer = null;
    private Pigeon prefabPigeon = null;
    private PigeonQueen prefabPigeonQueen = null;
    private PropBase prefabProp = null;

    public SceneGame(SCENES scene) : base(scene)
    {
    }

    public override bool Init(JSONObject param)
    {
        SoundManager.Instance.PlayMusic("17856_1462216818");
        
        menu = UIManager.Instance.OpenMenu<UIMenuGame>("UI/UIMenuGame");
        if(menu != null)
        {
            menu.InitMenu();
        }

        prefabBoat = ResourcesManager.Instance.LoadBundle<Boat>("Boat.prefab");
        prefabMagnatic = ResourcesManager.Instance.LoadBundle<Magnatic>("Magnatic.prefab");
        prefabPlayer = ResourcesManager.Instance.LoadBundle<Skell.Player>("Player.prefab");
        prefabPigeon = ResourcesManager.Instance.LoadBundle<Pigeon>("Pigeon.prefab");
        prefabPigeonQueen = ResourcesManager.Instance.LoadBundle<PigeonQueen>("PigeonQueen.prefab");
        prefabProp = ResourcesManager.Instance.LoadBundle<PropBase>("Prop.prefab"); 
        player = Instantiate<Skell.Player>(prefabPlayer);
        player.transform.position = Vector3.zero;

        boat = Instantiate<Boat>(prefabBoat);
        if (boat != null)
        {
            boat.GenerateNavmesh();
        }

        magnatic = Instantiate<Magnatic>(prefabMagnatic);
        if (magnatic != null)
        {
            magnatic.SetAngle(0);
        }

        monsters = new List<UnitBase>();

        StartCoroutine("Proc", gameObject);
        return true;
    }

    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;

        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10;
            Vector3 position = GeneratePosition(angle);

            Handles.Label(position, $"{angle}", style);
        }
    }
#endif

    private Vector3 GeneratePosition(float angle)
    {
        float r = 50;
        float x = (Mathf.Sin(angle * (Mathf.PI / 180)) * r);
        float z = (Mathf.Cos(angle * (Mathf.PI / 180)) * r);
        Vector3 position = new Vector3(x, 0, z);

        return position;
    }

    private System.Collections.IEnumerator Proc()
    {
        while (true)
        {
            if (monsters.Count < 100)
            {
                Vector3 position = GeneratePosition(UnityEngine.Random.Range(0, 360));
                PropBase mon = Instantiate<PropBase>(prefabProp);
                mon.transform.position = position;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void OnTouchBean(Vector3 position)
    {
        
    }

    public override void OnTouchEnd(Vector3 position)
    {
        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            var layer = hit.collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Tile"))
            {
                var agent = player.GetComponent<NavMeshAgent>();
                agent.SetDestination(hit.point);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                float angle = GetAngle(Vector3.zero, hit.point);
                boat.SetAngle(angle);

                menu.OnAngleChange(angle);
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
            Debug.Log(hit.point);
        }
    }

    public override void OnTouchMove(Vector3 position)
    {
        
    }
}
