using Common.Global;
using Common.Scene;
using Common.Table;
using Common.Utils;
using Creature;
using UI.Menu;
using UnityEngine;

public class SceneAI : SceneBase
{
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
            }
        }


        
        return true;
    }
}
