using Common.Global;
using Common.Scene;
using Common.Table;
using Common.Utils;
using Creature;
using UI.Menu;

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
                
                //var prefab = ResourcesManager.Instance.LoadBundle<Creature>("");
                var prefab = ResourcesManager.Instance.LoadBundle<Human>($"{data.PREFAB}");
                GiantDebug.Log($"{key} - {data.ID} {data.NAME_TID} {data.AGE} {data.LEVEL_GROUP_ID}");
            }
            //tableHero.Data.ForEach(data => Debug.Log($"{data.ID} {data.NAME_TID} {data.AGE} {data.ENABLE}"));
        }


        
        return true;
    }
}
