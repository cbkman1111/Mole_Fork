using System.Linq;
using Common.Global;
using Common.Scene;
using Common.Table;
using Common.Utils;
using Creature;
using UI.Menu;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static Creature.WorldObject;

public class SceneAI : SceneBase
{
    [SerializeField] private Transform Rocks;
    [SerializeField] private Transform Woods;
    [SerializeField] private Transform Creatrues;

    [SerializeField] private Vector3 offset;
    

    private WorldObjectList listCretures = new();
    private WorldObjectList listProbs = new();
    private int Width = 100;
    private int Height = 100;

    public override bool Init(JSONObject param)
    {
        DataManager.Instance.Load();

        var menu = UIManager.Instance.OpenMenu<UIMenuAI>();
        if (menu != null)
        {
            menu.InitMenu(OnClick);
        }

        CreatePineTree();
        CreateHero();
        CreateMonsters();

        var cretures = listCretures.List();
        var probs = listProbs.List();
        foreach (var obj in listCretures)
        {
            var worldObj = obj.GetComponent<WorldObject>();
            if (worldObj != null)
            {
                worldObj.BlackboardReference.SetVariableValue("Creatures", cretures);
                worldObj.BlackboardReference.SetVariableValue("Probs", probs);
            }
        }
        return true;
    }

    private void CreateHero()
    {
        WorldObject character = null;

        var tableHero = DataManager.Instance.Get<TableHero>();
        if (tableHero != null)
        {
            var first = tableHero.Data.First();
            var key = first.Key;
            var data = first.Value;

            var x = (int)UnityEngine.Random.Range(Width * -0.5f, Width * 0.5f);
            var z = (int)UnityEngine.Random.Range(Height * -0.5f, Height * 0.5f);

            WorldObjectCreateParam param;
            param.Path = $"{data.PREFAB}";
            param.Parent = Creatrues;
            param.X = x;
            param.Z = z;
            param.ObjectTeam = ObjectTeam.Enemy;
            
            character = WorldObject.Create(param);
            if (character == null)
                return;

            listCretures.Add(character);
        }

        //int index = UnityEngine.Random.Range(0, listCretures.Count);
        SetCameraTarget(character.transform);
    }

    private void CreateMonsters()
    {
        var tableHero = DataManager.Instance.Get<TableHero>();
        if (tableHero != null)
        {
            foreach (var pair in tableHero.Data)
            {
                var data = pair.Value;
                var key = pair.Key;
                var x = (int)UnityEngine.Random.Range(Width * -0.5f, Width * 0.5f);
                var z = (int)UnityEngine.Random.Range(Height * -0.5f, Height * 0.5f);

                WorldObjectCreateParam param;
                param.Path = $"{data.PREFAB}";
                param.Parent = Creatrues;
                param.X = x;
                param.Z = z;
                param.ObjectTeam = ObjectTeam.Enemy;

                var character = WorldObject.Create(param);
                if (character == null)
                    continue;

                listCretures.Add(character);
            }
        }

        int index = UnityEngine.Random.Range(0, listCretures.Count);
        SetCameraTarget(listCretures[index].transform);
    }

    private void CreatePineTree()
    {
        for (int i = 0; i < 1000; i++)
        {
            var x = (int)UnityEngine.Random.Range(Width * -0.5f, Width * 0.5f);
            var z = (int)UnityEngine.Random.Range(Height * -0.5f, Height * 0.5f);
            
            WorldObjectCreateParam param;
            param.Path = $"Assets/AddressableAssets/Prefab/Prob/PineTree.prefab";
            param.Parent = Woods;
            param.X = x;
            param.Z = z;
            param.ObjectTeam = ObjectTeam.Enemy;

            var tree = WorldObject.Create(param);
            if (tree == null)
                return;

            tree.name = $"PineTree_{i}";
            listProbs.Add(tree);
        }

        /*
        var listGameObject = listProbs.List();
        foreach (var obj in listProbs)
        {
            var worldObj = obj.GetComponent<WorldObject>();
            if (worldObj != null)
            {
                worldObj.BlackboardReference.SetVariableValue("Probs", listGameObject);
            }
        }
        */
    }

    private void SetCameraTarget(Transform target)
    {
        int index = UnityEngine.Random.Range(0, listCretures.Count);
        var x = target.position.x;
        var z = target.position.z;
        MainCamera.transform.position = new UnityEngine.Vector3(x + offset.x, offset.y, z + offset.z);
    }

    private void OnClick(string name)
    {
        if (name == "Button - 1")
        { 
            GiantDebug.Log($"listCretures Count : {listCretures.Count}");
            GiantDebug.Log($"listProbs Count : {listProbs.Count}");

            int index = UnityEngine.Random.Range(0, listCretures.Count);
            SetCameraTarget(listCretures[index].transform);

        }
        else if (name == "Button - 2")
        {
        }
        else if (name == "Button - 3")
        {
        }
        else if (name == "Button - 4")
        {
        }
        else if (name == "Button - 5")
        {
        }
        else if (name == "Button - 6")
        {
        }
    }
}
