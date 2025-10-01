using System.Linq;
using Common.Global;
using Common.Scene;
using Common.Table;
using Creature;
using UI.Menu;

public class SceneAI : SceneBase
{
    private WorldObjectList list = new();

    public override bool Init(JSONObject param)
    {
        var menu = UIManager.Instance.OpenMenu<UIMenuAI>();
        if (menu != null)
        {
            menu.InitMenu();
        }

        DataManager.Instance.Load();

        var tableHero = DataManager.Instance.Get<TableHero>();
        if (tableHero != null)
        {
            foreach (var pair in tableHero.Data)
            {
                var data = pair.Value;
                var key = pair.Key;
                var x = UnityEngine.Random.Range(-5, 5);
                var z = UnityEngine.Random.Range(-5, 5);
                var character = WorldObject.Create($"{data.PREFAB}", null, x, z);
                if (character == null)
                {
                    continue;
                }

                list.Add(character);
            }
        }

        var listGameObject = list.List();
        foreach (var obj in list)
        {
            var worldObj = obj.GetComponent<WorldObject>();
            if (worldObj != null)
            {
                worldObj.BlackboardReference.SetVariableValue("Creatures", listGameObject);
            }
        }

        return true;
    }
}
