using Common.Global;
using Common.Scene;
using Common.Utils;
using Games.TileMap;
using Poker;
using Scenes.EllersAlgorithm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UI.Menu;
using Unity.Mathematics;

namespace Scenes
{
    /// <summary>
    /// 경기 도중 판돈을 올리는 과정. 재미삼아 가볍게 하는 포커라면 큰 문제가 안 되지만, 
    /// 진지하게 하는 포커 게임에선 가장 중요한 과정이다. 
    /// 베팅에 따라 상대가 어떤 핸드를 들고 있는지 파악할 수 있으며, 이런 예측을 역으로 노리는 베팅 방식이 '블러핑'이다.[6]
    /// 
    /// 공식 베팅 용어
    /// 폴드(Fold)    : 경기를 포기하는 것.포기하기 전까지 베팅한 금액은 잃게 된다.한국에서는 수입 과정에서 명칭이 죽는다는 뜻의 '다이(Die)'로 바뀌었다.[7]
    /// 체크(Check)   : 판돈을 추가하지 않고 차례를 넘기겠다는 신호. 카드가 돌아가고 처음으로 베팅하는 사람만 쓸 수 있다. 단, 이후 다른 사람이 체크를 받아들이지 않고 판돈을 올렸다면 체크를 한 사람도 콜/레이즈를 하거나 판을 포기해야 한다.
    /// 베트(Bet) : 한 베팅 라운드에서 최초로 판돈을 올리겠다는 신호.
    ///     만약 아무도 베트를 선언하지 않으면[8] 플레이어 전원이 이번 라운드에선 모두 판돈을 올릴 의사가 없다고 간주되어 추가베팅 없이 다음 단계로 넘어가게 된다.
    ///     베트를 선언한 사람은 그만큼의 베팅액을 올려야 하며, 다른 플레이어들은 콜 혹은 레이즈를 선언해야만 계속해서 게임을 진행할 수 있다.
    /// 콜(Call) : 앞의 플레이어가 판돈을 올린 것을 받아들인다는 의미.
    ///     하나의 베트 · 레이즈에 모든 플레이어가 콜하면, 베팅 라운드는 다음 과정으로 넘어간다.
    /// 레이즈(Raise)[9]   : 앞의 플레이어가 판돈을 올린 것을 받아들이고, 또한 거기서 추가로 더 베팅하는 것. 
    ///     한 베팅에 최대 2~3번까지 가능한 것이 일반적이지만, 레이즈 한도를 없앤 노리밋(No Limit) 룰도 있다.
    ///     이 때 올인이라는 단어를 매우 쉽게 볼 수 있다.[10]
    /// 올인(All in): 자신이 현재 테이블 위에 보유한 모든 칩을 모두 베팅하는 것.
    ///     노 리밋 포커의 꽃이다.
    ///     상대방을 이길 수 있다는 확신이 있거나 상대방에게 자신이 확실하게 이길 자신이 있다는 블러핑을 할 때, 
    ///     혹은 어차피 빈털터리라서 상관없을 때 쓰인다. 
    ///     상대방의 레이즈를 콜할 금액이 없어서 남은 칩이라도 베팅할 때도 올인이라고 한다.
    ///     다만 상대방보다 적은 금액으로 콜을 하고 게임에서 이길 경우에는 각각의 플레이어들에게서 자신이 베팅한 금액을 초과하는 금액은 받을 수 없다.
    ///     이 때 발생한 사이드머니는 카지노에 따라 처리하는 방법이 다른데 각자 베팅머니에서 승자의 베팅머니를 뺀 나머지만큼 각자에게 돌려주는 방식과 차상위 패를 가진 자에게 사이드머니를 모두 주는 방식이 있다.
    ///     언제 선언하든 그 시점에서 올인한 사람은 추가 액션을 취할 수 없기에 쇼다운까지 그 사람의 차례는 건너뛴다.
    ///
    /// 한국에서만 쓰이는 하우스 룰은 아래와 같다. 
    /// 이는 한국의 포커가 독자적인 방향으로 발달하면서 생긴 베팅 규칙으로, 
    /// 공식적으로는 사용되지 않는 개념이므로 주의. 베트나 레이즈에서 얼마만큼 걸지에 대한 특별한 룰은 없다.[11][12]
    /// 한국에서는 체크나 콜을 한 플레이어의 리레이즈를 허용하지 않고, 
    /// 콜과 다이만 허용하는 경우가 많다.
    /// 
    /// 서양과 달리 한국에서는 체크-레이즈와 콜-레이즈를 매우 나쁘게 보는 탓에 온라인 포커에서도 이를 금지하는 경우가 많다.
    /// 삥: 시드만큼 베팅하는 것.한국에서는 보통 체크 후 레이즈를 허용하지 않기 때문에 이를 피하기 위해 최소한의 돈을 거는 것이다.
    /// 따당: 상대가 건 돈의 2배를 베팅하는 것.
    /// 쿼터(quarter): 쌓인 판돈의 4분의 1을 베팅하는 것.
    /// 하프(half): 쌓인 판돈의 절반을 베팅하는 것.
    /// 풀(full): 쌓인 판돈만큼 베팅하는 것.
    /// 맥스(max): 최대치로 베팅하는 것.
    /// </summary>
    public class ScenePoker : SceneBase
    {
        public enum BetType
        {
            Fold,
            Check,
            Bet,
            Call,
            Raise,
            AllIn,
        }

        public enum PlayUser
        {
            Player1 = 0,
            Player2,
            Player3,
            Player4,
            Player5,

            Max,
        }

        public enum HandRank
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            RoyalFlush
        }

        public Deck Deck = null;
        public Dictionary<PlayUser, Player> Players = null;

        private UIMenuPoker menu = null;

        /// <summary>
        /// 포커 초기화.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            // 1. 플레이어 추가
            Players = new();
            Players.Add(PlayUser.Player1, new Player());
            Players.Add(PlayUser.Player2, new Player());
            Players.Add(PlayUser.Player3, new Player());
            Players.Add(PlayUser.Player4, new Player());
            Players.Add(PlayUser.Player5, new Player());

            // 2. 카드 덱 초기화
            Deck = new();
            Deck.Shuffle();

            // 3. 카드 배분
            for (PlayUser player = 0; player < PlayUser.Max; player++)
            {
                if(Players.TryGetValue(player, out var playerInfo))
                {
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());
                    playerInfo.AddCard(Deck.DeQueue());

                    GiantDebug.Log($"Player {player} : {playerInfo.Hand.ToString()}");
                }
            }

            // 4. UI 초기화
            menu = UIManager.Instance.OpenMenu<UIMenuPoker>();
            if (menu != null)
            {
                menu.InitMenu(this);
            }

            CheckHands();

            return true;
        }

        public void CheckHands()
        {
            menu.UpdateRank(this);

        }

        private void OnStartGame()
        { 

        }
    }
}