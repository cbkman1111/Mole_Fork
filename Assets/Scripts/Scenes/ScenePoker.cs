using Common.Global;
using Common.Scene;
using UI.Menu;

namespace Scenes
{
    /// <summary>
    /// https://m-poker.hangame.com/guide/rule
    /// 
    /// 7 포커
    /// 1. 3장의 카드 받고 => 뒤집기 1장 오픈
    /// 2. 4번째 카드 받기 => 1차 베팅(4구)
    /// 3. 5번째 카드 받기 => 2차 베팅(5구)
    /// 4. 6번째 카드 받기 => 3차 베팅(6구)
    /// 5. 히든카드 받기
    /// 6. 최종 4차 베팅(7구)
    /// 7. 히든 오픈
    /// 
    /// 
    /// * 베팅 방법
    /// 콜   : 앞 사람의 베팅 금액과 동일한 베팅 금액을 겁니다.
    /// 삥   :기본 판돈만 베팅합니다.(게임방 보스만 가능)
    /// 따당 : 앞사람이 베팅한 금액의 2배를 베팅합니다.
    /// 하프 : 전체 판돈의 절반, 즉 50% 금액을 베팅합니다.
    /// 다이 :  새로 베팅하지 않고, 이번 판을 포기합니다.
    /// 체크 : 머니를 베팅하지 않고 다음 카드를 받습니다.(보스만 가능)
    /// 쿼터 : 전체 판돈의 1/4, 즉 25% 금액을 베팅합니다.
    /// 풀   : 전체 판돈만큼, 판돈의 100% 금액을 베팅 합니다.
    /// 맥스 : 자신의 베팅 한도내의 최대 베팅(최대 38.5억/친구경기장 최대 1.925억)
    /// </summary>
    public class ScenePoker : SceneBase
    {
        private UIMenuPoker menu = null;

        /// <summary>
        /// 포커 초기화.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            long[] players = { 10000001 , 10000002, 10000003, 10000004, 10000005, 0, 0, 0, 0, 0}; // PlayUser.Max
            var gameData = GlobalGameManager.Instance.CreatePokerGame(players);

            // 4. UI 초기화
            menu = UIManager.Instance.OpenMenu<UIMenuPoker>();
            if (menu != null)
            {
                menu.InitMenu(OnStartGame);
            }

            menu.UpdateUI();
            return true;
        }

        private void OnStartGame()
        {
            long[] players = { 10000001, 10000002, 10000003, 10000004, 10000005, 0, 0, 0, 0, 0 }; // PlayUser.Max
            var gameData = GlobalGameManager.Instance.CreatePokerGame(players);
            menu.InitMenu(OnStartGame);
        }

    }
}