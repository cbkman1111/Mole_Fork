using System.Collections.Generic;
using UnityEngine;
using static UnitBase;

public class SceneGame : SceneBase
{
    private Player player = null;
    private List<UnitBase> monsters = null;

    public SceneGame(SCENES scene) : base(scene)
    {
    }

    public override bool Init()
    {
        SoundManager.Instance.PlayMusic("17856_1462216818");
        
        UIGameMenu menu = UIManager.Instance.OpenMenu<UIGameMenu>("UIGameMenu");
        if(menu != null)
        {
            menu.InitMenu();
        }

        Player prefabPlayer = ResourcesManager.Instance.LoadBundle<Player>("Player.prefab");
        Pigeon prefabPigeon = ResourcesManager.Instance.LoadBundle<Pigeon>("Pigeon.prefab");
        PigeonQueen prefabPigeonQueen = ResourcesManager.Instance.LoadBundle<PigeonQueen>("PigeonQueen.prefab");

        player = Instantiate<Player>(prefabPlayer);
        player.transform.position = Vector3.zero;

        monsters = new List<UnitBase>();
        for ( int i = 0; i < 20; i++)
        {
            UnitBase monster = null;
            if(i == 0)
            {
                monster = Instantiate<PigeonQueen>(prefabPigeonQueen);
            }
            else
            {
                monster = Instantiate<Pigeon>(prefabPigeon);
            }

            int x = UnityEngine.Random.Range(0, 10) - 5;
            int z = UnityEngine.Random.Range(0, 10) - 5;
            int y = 0;
            
            monster.transform.position = new Vector3(x, y, z);

            SkellAnimationState[] animations = { SkellAnimationState.idle, SkellAnimationState.run, SkellAnimationState.die, SkellAnimationState.attack};
            int randIndex = UnityEngine.Random.Range(0, animations.Length);
            string name = animations[randIndex].ToString();
            monster.skel.state.SetAnimation(0, name, true);
            monsters.Add(monster);
        }


        //player = Instantiate<PigeonQueen>(prefabPigeonQueen);
        //player.transform.position = Vector3.zero;
        return true;
    }

    public override void OnTouchBean(Vector3 position)
    {
        
    }

    public override void OnTouchEnd(Vector3 position)
    {
        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
            Debug.Log(hit.point);
        }
    }

    public override void OnTouchMove(Vector3 position)
    {
        
    }
}
