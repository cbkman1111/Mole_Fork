using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Match3
{
    public class Scene3Match : SceneBase
    {
        [SerializeField]
        private CandyBase[] CandyPrefabs = null;
        [SerializeField]
        private GameObject[] TileBG = null;
        [SerializeField]
        private GameObject TileRoot = null;
        
        private PoolCandy<CandyBase> PoolCandys = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuMatch3 menu = UIManager.Instance.OpenMenu<UIMenuMatch3>("UIMenuMatch3");
            if (menu != null)
            {
                menu.InitMenu();
                menu.OnStartGame = (int level) => {

                };
            }

            PoolCandys = new();
            PoolCandys.InitList(CandyPrefabs);
            return true;
        }

    }
}
