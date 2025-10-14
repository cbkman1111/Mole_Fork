using System.Linq;
using Common.Global;
using Common.Scene;
using Common.Table;
using Common.Utils;
using Creature;
using UI.Menu;
using UnityEngine;

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
        CreateHeros();

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

    private void CreateHeros()
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
                var character = WorldObject.Create($"{data.PREFAB}", null, x, z);
                if (character == null)
                {
                    continue;
                }
                character.transform.SetParent(Creatrues);
                listCretures.Add(character);
            }
        }

      

        int index = UnityEngine.Random.Range(0, listCretures.Count);
        SetCameraTarget(listCretures[index].transform);
    }

    private void CreatePineTree()
    {
        for (int i = 0; i < 500; i++)
        {
            var x = (int)UnityEngine.Random.Range(Width * -0.5f, Width * 0.5f);
            var z = (int)UnityEngine.Random.Range(Height * -0.5f, Height * 0.5f);
            var tree = WorldObject.Create($"Assets/AddressableAssets/Prefab/Prob/PineTree.prefab", null, x, z);
            if (tree == null)
                return;

            tree.transform.SetParent(Woods);
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
