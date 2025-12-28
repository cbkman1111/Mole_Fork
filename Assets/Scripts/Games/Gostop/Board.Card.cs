using Common.Global;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Popup;
using UnityEngine;

namespace Gostop
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public Stack<Card> deck = null;

        public CardList[] hands = null;
        public CardList[] scores = null;

        public Dictionary<int, CardList> bottoms = null;
        
        private CardList select = null; // 선택해야 하는 카드.
        private CardList listEat = null; // 먹는패.

        private void StealCard()
        {
            if (stealCount <= 0) 
                return;

            // 1. 타겟 설정
            int targetIndex = (turnUser == Player.Me) ? (int)Player.Enemy : (int)Player.Me;
            CardList targetScoreList = scores[targetIndex];

            // 2. 피 종류별 분류 (1피, 2피, 3피)
            // 리스트를 복사해서 사용하는 것이 안전함 (RemoveAt 할 때 인덱스 문제 방지)
            var p1List = targetScoreList.Where(c => c.IsPe && c.PeCount == 1).ToList();
            var p2List = targetScoreList.Where(c => c.IsPe && c.PeCount == 2).ToList();
            var p3List = targetScoreList.Where(c => c.IsPe && c.PeCount == 3).ToList();

            List<Card> cardsToSteal = new List<Card>();

            // ==========================================================
            // 핵심 로직: 1피로 해결 가능한가?
            // ==========================================================

            // [Case 1] 1점짜리만으로 충분히 낼 수 있는 경우 (무조건 1점짜리로만 냄)
            if (p1List.Count >= stealCount)
            {
                for (int i = 0; i < stealCount; i++)
                {
                    cardsToSteal.Add(p1List[i]);
                }
            }
            // [Case 2] 1점짜리가 모자란 경우 (어쩔 수 없이 큰 것부터 내서 퉁침)
            else
            {
                // 남은 뺏을 수량
                int remain = stealCount;

                while (remain > 0)
                {
                    // 3점(쓰리피)이 있고, 남은 뺏을 양이 많으면 우선 처리
                    if (p3List.Count > 0)
                    {
                        var card = p3List[0];
                        cardsToSteal.Add(card);
                        p3List.RemoveAt(0);
                        remain -= 3;
                    }
                    // 2점(쌍피)이 있으면 처리
                    else if (p2List.Count > 0)
                    {
                        var card = p2List[0];
                        cardsToSteal.Add(card);
                        p2List.RemoveAt(0);
                        remain -= 2;
                    }
                    // 1점(피)이 있으면 처리
                    else if (p1List.Count > 0)
                    {
                        var card = p1List[0];
                        cardsToSteal.Add(card);
                        p1List.RemoveAt(0);
                        remain -= 1;
                    }
                    // 상대방 피가 다 말랐음
                    else
                    {
                        break;
                    }
                }
            }

            // 3. 실제 이동 및 제거 처리
            foreach (var card in cardsToSteal)
            {
                targetScoreList.Remove(card);
                TackCard(card); // 내 패로 가져오는 함수 (비동기 연출 등 포함 가능)
                break;
            }

            stealCount = 0;
        }

        /// <summary>
        /// 기존 카드를 제거합니다.
        /// </summary>
        private void DestroyAllCards()
        {
            // 객체 지우기.
            for (int i = 0; i < deck.Count(); i++)
            {
                var card = deck.Pop();
                DOTween.KillAll(card.gameObject);
                GameObject.Destroy(card.gameObject);
            }

            foreach (var kindSlot in bottoms)
            {
                kindSlot.Value.Destroy();
            }

            for (int i = 0; i < (int)Player.Max; i++)
            {
                hands[i].Destroy();
            }

            for (int i = 0; i < (int)Player.Max; i++)
            {
                foreach (var card in scores[i])
                {
                    DOTween.KillAll(card.gameObject);
                    GameObject.Destroy(card.gameObject);
                }
            }

            // 배열 지우기.
            deck.Clear();
            commandProcedure.Clear();

            for (int i = 0; i < (int)Player.Max; i++)
            {
                scores[i].Clear();
            }
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<T> ShuffleList<T>(List<T> list)
        {
            int random1, random2;
            T temp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = UnityEngine.Random.Range(0, list.Count);
                random2 = UnityEngine.Random.Range(0, list.Count);

                temp = list[random1];
                list[random1] = list[random2];
                list[random2] = temp;
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HandSort()
        {
            try 
            {
                for (int i = 0; i < (int)Player.Max; i++)
                {
                    hands[i].OrderByNum();
                    for (int index = 0; index < hands[i].Count; index++)
                    {
                        var card = hands[i][index];
                        var handCard = boardPositions[i].Hand.GetChild(index);
                        var handPosition = handCard.position;
                        var handScale = handCard.localScale;
                        card.MoveTo(
                            handPosition,
                            handScale,
                            time: 0.1f);
                    }

                }
            } 
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return true;
        }

        /// <summary>
        /// 스코어 처리.
        /// </summary>
        private void ScoreUpdate()
        {
            for (int user = 0; user < (int)Player.Max; user++)
            {
                int gwang = scores[user].Where(card => card.KindOfCard == Card.KindOf.GWANG || card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
                int mung = scores[user].Where(card => card.KindOfCard == Card.KindOf.MUNG || card.KindOfCard == Card.KindOf.MUNG_GODORI || card.KindOfCard == Card.KindOf.MUNG_KOO).ToList().Count();
                int thee = scores[user].Where(card => card.KindOfCard == Card.KindOf.CHO || card.KindOfCard == Card.KindOf.CHUNG || card.KindOfCard == Card.KindOf.HONG).ToList().Count();

                // 광점수
                if (gwang == 5)
                {
                    gameScore[user].gawng = 15;
                }
                else if (gwang == 4)
                {
                    gameScore[user].gawng = 4;
                }
                else if (gwang == 3)
                {
                    int bgwang = scores[user].Where(card => card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
                    gameScore[user].gawng = bgwang == 1 ? 2 : 3;
                }

                // 띠점수
                if (thee >= 5)
                {
                    gameScore[user].thee = thee - 4;

                    if (scores[user].Where(card => card.KindOfCard == Card.KindOf.CHO).ToList().Count == 3)
                    {
                        gameScore[user].thee += 3;
                        gameScore[user].chodan = true;
                    }

                    if (scores[user].Where(card => card.KindOfCard == Card.KindOf.CHUNG).ToList().Count == 3)
                    {
                        gameScore[user].thee += 3;
                        gameScore[user].chungdan = true;
                    }

                    if (scores[user].Where(card => card.KindOfCard == Card.KindOf.HONG).ToList().Count == 3)
                    {
                        gameScore[user].thee += 3;
                        gameScore[user].hongdan = true;
                    }
                }

                // 멍점수
                if (mung >= 5)
                {
                    gameScore[user].mung = mung - 4;
                    if (scores[user].Where(card => card.KindOfCard == Card.KindOf.MUNG_GODORI).ToList().Count == 3)
                    {
                        gameScore[user].mung += 5;
                        gameScore[user].godori = true;
                    }
                }

                // 피점수
                var list = scores[user].Where(
                    card => card.KindOfCard == Card.KindOf.P || 
                    card.KindOfCard == Card.KindOf.PP || 
                    card.KindOfCard == Card.KindOf.PPP).ToList();

                int pee = 0;
                foreach (var card in list)
                {
                    switch (card.KindOfCard)
                    {
                        case Card.KindOf.P:
                            pee += 1;
                            break;
                        case Card.KindOf.PP:
                            pee += 2;
                            break;
                        case Card.KindOf.PPP:
                            pee += 3;
                            break;
                    }
                }

                if (pee >= 10)
                {
                    gameScore[user].pee = pee - 9;
                }
            }

            // 박 계산
            for (int user = 0; user < (int)Player.Max; user++)
            {
                int player = (int)user;
                int enemy = (int)Player.Enemy;
                if (player == (int)Player.Me)
                {
                    enemy = (int)Player.Enemy;
                }
                else
                {
                    enemy = (int)Player.Me;
                }

                if (gameScore[player].gawng > 0)
                {
                    int gwang = scores[enemy].Where(card => card.KindOfCard == Card.KindOf.GWANG || card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
                    if (gwang == 0)
                    {
                        gameScore[player].gwangbak = true;
                    }
                    else
                    {
                        gameScore[player].gwangbak = false;
                    }
                }

                if (gameScore[player].pee > 0)
                {
                    int pee = 0;
                    var list = scores[enemy].Where(card => card.KindOfCard == Card.KindOf.P || card.KindOfCard == Card.KindOf.PP || card.KindOfCard == Card.KindOf.PPP).ToList();
                    foreach (var card in list)
                    {
                        switch (card.KindOfCard)
                        {
                            case Card.KindOf.P:
                                pee += 1;
                                break;
                            case Card.KindOf.PP:
                                pee += 2;
                                break;
                            case Card.KindOf.PPP:
                                pee += 3;
                                break;
                        }
                    }

                    if (pee < 6)
                    {
                        gameScore[player].peebak = true;
                    }
                    else
                    {
                        gameScore[player].peebak = false;
                    }
                }


                if (gameScore[player].mung >= 7)
                {
                    gameScore[player].mungbak = true;
                }
                else
                {
                    gameScore[player].mungbak = false;
                }

                gameScore[player].total = gameScore[player].gawng + gameScore[player].mung + gameScore[player].thee + gameScore[player].pee + gameScore[player].go;
                int multiCount = 0;
                if (gameScore[player].peebak == true)
                {
                    multiCount += 1;
                }
                if (gameScore[player].gwangbak == true)
                {
                    multiCount += 1;
                }
                if (gameScore[player].mungbak == true)
                {
                    multiCount += 1;
                }
                if (gameScore[player].go >= 3)
                {
                    multiCount += gameScore[player].go - 3;
                }

                float muti = Mathf.Pow(2, multiCount);
                gameScore[player].total *= (int)muti;
                updateScore((Player)user, gameScore[user]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CreateDeck()
        {
            List<int> nums = new List<int>();
            int cardCount = 50;
            for (int i = 0; i < cardCount; i++)
            {
                nums.Add(i + 1);
            }

            nums = ShuffleList(nums);

            for (int i = 0; i < nums.Count; i++)
            {
                int n = nums[i];
                Card card = Instantiate<Card>(prefabCard);
                if (card != null)
                {
                    Sprite sprite = sprites[n - 1];
                    card.Init(n, sprite);
                    deck.Push(card);

                    float height = card.Height;
                    var dest = new Vector3(Deck.x, Deck.y + height * i, Deck.z);

                    card.transform.position = Deck;
                    card.SetOpen(false);
                    card.MoveTo(dest, Vector3.one, time: setting.DeckCardTime, delay: i * 0.01f);
                }
            }

            return true;
        }

        /// <summary>
        /// 바닥 8장 뿌리기.
        /// </summary>
        /// <returns></returns>
        public void Shuffle8Card()
        {
            for (int i = 0; i < 8; i++)
            {
                Card card = deck.Pop();

                KeyValuePair<int, CardList> slot = GetSlot(card);
                float randX = UnityEngine.Random.Range(0.2f, 0.3f);
                float randZ = UnityEngine.Random.Range(0.1f, 0.15f);

                var stack = slot.Value.Count;
                var y = card.Height * stack;
                Vector3 position = cardPosition[slot.Key - 1].position + new Vector3(i * randX, y, i * randZ);
  
                card.MoveTo(
                    position,
                    Vector3.one,
                    time: setting.SuffleCardTime,
                    delay: i * setting.SuffleCardInterval, 
                    complete: () => {
                        card.SetEnablePhysics(true);
                    });

                card.CardOpen(0.1f);
                slot.Value.Add(card);
            }
        }

        /// <summary>
        /// 10장씩 나눠주기
        /// </summary>
        /// <returns></returns>
        public bool Shuffle10Card()
        {
            for (int user = 0; user < 2; user++)
            {
                for (int i = 0; i < 10; i++)
                {
                    Card card = deck.Pop();

                    float randX = UnityEngine.Random.Range(-1.00f, 1.0f);
                    float randZ = UnityEngine.Random.Range(1.00f, 1.0f);
                    var recivePosition = boardPositions[user].RecvieCard.position;
                    Vector3 position = recivePosition + new Vector3(randX, i * card.Height, randZ);
                    
                    card.MoveTo(
                        position,
                        Vector3.one,
                        time: setting.SuffleCardTime,
                        delay: user * 0.2f + i * setting.SuffleCardInterval);

                    card.Owner = (Player)user;
                    hands[user].Add(card);
                }
            }

            return true;
        }

        public int CheckJoker()
        {
            int count = 0;
            foreach (var slot in bottoms)
            {
                var list = slot.Value.GetList(13);
                if (list == null || list.Count == 0)
                    continue;

                for (int i = list.Count - 1; i >= 0; --i)
                {
                    var card = list[i];
                    TackCard(card, list.Count - i);
                    slot.Value.Remove(card);
                    
                    count++;
                    break;
                }

                if (count > 0)
                {
                    break;
                }
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool FlipCard8()
        {
            foreach (var slot in bottoms)
            {
                var cardList = slot.Value;

                foreach (var card in cardList)
                {
                    card.CardOpen(setting.FlipTime);
                    card.SetEnablePhysics(true);
                }
            }

            return true;
        }

        /// <summary>
        /// 손패를 받아 듭니다.
        /// </summary>
        /// <returns></returns>
        public bool HandsUp()
        {
            for (int user = 0; user < 2; user++)
            {
                var list = hands[user];
                list.Reverse();

                for (int i = 0; i < list.Count; i++)
                {
                    var card = list[i];
                    var slot = boardPositions[user].Hand.transform.GetChild(i);
                    var rotate = boardPositions[user].Hand.rotation;
                    card.CardOpen(setting.FlipTime);
                    card.MoveTo(
                        slot.position,
                        slot.localScale,
                        time: setting.HandUpTime,
                        delay: i * setting.HandUpDelay);
                }
            }

            return true;
        }

        /// <summary>
        /// 손패를 뒤집습니다.
        /// </summary>
        /// <returns></returns>
        private bool HandOpen()
        {
            for (int index = 0; index < hands[(int)Player.Me].Count; index++)
            {
                Card card = hands[(int)Player.Me][index];
                card.ShowMe(delay: index * 0.2f);
                //card.SetShadow(false);
            }

            for (int index = 0; index < hands[(int)Player.Enemy].Count; index++)
            {
                Card card = hands[(int)Player.Enemy][index];
                card.SetOpen(true);
                //card.SetShadow(false);
            }

            return true;
        }

        /// <summary>
        /// 손에 보유한 카드 수량을 리턴합니다.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public List<Card> GetSameMonthCard(int user, Card card)
        {
            return hands[user].SameList(card);// Where(c => c.Month == card.Month && c.Month != 13).ToList();
        }

        /// <summary>
        /// 총통 처리.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="list"></param>
        public void HitChongtong(int user, List<Card> list, Card selected)
        {
            HitCard(user, selected, 0.2f);
            gameScore[user].shake += 1;
        }

        /// <summary>
        /// 폭탄.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="list"></param>
        public void HitBomb(int user, List<Card> list, Card select)
        {
            bool eatPossible = false;

            foreach (KeyValuePair<int, CardList> kindSlot in bottoms)
            {
                if (kindSlot.Value.Count > 0)
                {
                    if (kindSlot.Value[0].Month == list[0].Month)
                    {
                        eatPossible = true;
                        break;
                    }
                }
            }

            if (eatPossible == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    HitCard(user, list[i], i * 0.2f);

                    // 폭탄 카드를 손에 쥐어줍니다.
                    if (i > 0)
                    {
                        //string pathCard = "CardGostop.prefab";
                        Card card = Instantiate<Card>(prefabCard);
                        //Card card = ResourcesManager.Instance.InstantiateInBuild<Card>(pathCard);
                        if (card != null)
                        {
                            card.Init(52, spriteBomb);
                            card.transform.position = list[i].transform.position;
                            card.transform.rotation = list[i].transform.rotation;
                            hands[user].Add(card);
                        }
                    }
                }

                stealCount += 1;
                gameScore[user].shake += 1;
            }
            else
            {
                HitCard(user, select, 0.2f);
            }
        }

        private void ExplosionEffect(Card centerCard)
        {
            float radius = 2f;        // 탐색 반경
            float power = 0.5f;       // 밀려나는 거리 (강도)
            float duration = 0.5f;    // 흔들리는 시간
            /*
            // 1. 주변 카드 탐색
            Collider[] colliders = Physics.OverlapSphere(centerCard.transform.position, radius);

            foreach (var coll in colliders)
            {
                // 자기 자신 제외하고 Card 컴포넌트 가져오기
                if (coll.TryGetComponent(out Card surroundCard) && surroundCard != centerCard)
                {
                    // [수정됨] "같은 월(Month)"인 경우에만 흔들리도록 조건 추가
                    if (surroundCard.Month == centerCard.Month)
                    {
                        // 2. 밀려날 방향 계산
                        Vector3 direction = surroundCard.transform.position - centerCard.transform.position;

                        if (direction.magnitude < 0.01f)
                            direction = UnityEngine.Random.onUnitSphere;

                        // 3. 펀치 효과 실행
                        Vector3 punchVector = direction.normalized * power;

                        surroundCard.transform.DOKill();
                        surroundCard.transform.DOPunchPosition(punchVector, duration, 10, 1f);
                    }
                }
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        public void HitCard(int user, Card card, float delay = 0)
        {
            var playInfo = CommandInfo.Info;
     
            KeyValuePair<int, CardList> slot = GetSlot(card);
            bool success = hands[user].Remove(card);
            if (success == true)
            {
                if (slot.Key != -1)
                {
                    float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
                    float randZ = UnityEngine.Random.Range(-0.5f, 0.5f);
                    int stackCount = slot.Value.Count;

                    Vector3 destination1 = hitPosition[user].position;
                    Vector3 destination2 = cardPosition[slot.Key - 1].position +
                        new Vector3(randX, stackCount * card.Height, randZ);

                    //card.SetShadow(true);
               
                    if (card.Month == 13) // 조커 카드.
                    {
                        playInfo.isHit = true;
                        playInfo.hit = card;
                        stealCount += 1;

                        TackCard(card, 1); // 카드 획득.
                    }
                    else if (card.Month == 14) // 폭탄 공짜 카드.
                    {
                        playInfo.isHit = true;
                        playInfo.hit = card;

                        GameObject.Destroy(card.gameObject);
                        commandProcedure.Enqueue(Command.PopCardDeck, turnUser);
                    }
                    else // 일반 카드.
                    {
                        playInfo.isHit = true;
                        playInfo.hit = card;
                        
                        slot.Value.Add(card);
                        card.MoveTo( // 카드를 위로 뽑아서.
                            destination1,
                            Vector3.one * 2,
                            time: setting.HitUpTime,
                            ease: DG.Tweening.Ease.InExpo,
                            complete: () => {

                                card.MoveTo(
                                    destination2,
                                    Vector3.one,
                                    time: setting.HitDownTime,
                                    ease: DG.Tweening.Ease.InExpo,
                                    delay: delay,
                                    complete: () =>
                                    {
                                        card.SetEnablePhysics(true);
                                        ExplosionEffect(card);
                                    });
                            });
                    }
                }
            }
        }

        /// <summary>
        /// 카드를 점수판으로 이동시킵니다.
        /// </summary>
        /// <param name="card"></param>
        private void TackCard(Card card, int count = 0, Action complete = null)
        {
            int user = (int)turnUser;
            List<Card> list = null; // 현재 해당 슬롯에 이미 있는 카드들
            Vector3 start = Vector3.zero; // 기준점 (슬롯의 0번 위치)

            // 카드 간격 설정 (카드의 너비 대비 몇 %씩 겹칠 것인가)
            float spacingRatio = 0.0f;

            switch (card.KindOfCard)
            {
                // [광] : 개수가 적으므로 넓게 배치 (80% 간격)
                case Card.KindOf.GWANG:
                case Card.KindOf.GWANG_B:
                    list = scores[user].Where(e => e.KindOfCard == Card.KindOf.GWANG || e.KindOfCard == Card.KindOf.GWANG_B).ToList();
                    start = boardPositions[user].Gwang.position;
                    spacingRatio = 0.8f;
                    break;

                // [멍/띠] : 적당히 겹침 (40% 간격)
                case Card.KindOf.MUNG:
                case Card.KindOf.MUNG_GODORI:
                case Card.KindOf.MUNG_KOO:
                    list = scores[user].Where(e => e.KindOfCard == Card.KindOf.MUNG || e.KindOfCard == Card.KindOf.MUNG_GODORI || e.KindOfCard == Card.KindOf.MUNG_KOO).ToList();
                    start = boardPositions[user].Mung.position;
                    spacingRatio = 0.45f;
                    break;

                case Card.KindOf.CHO:
                case Card.KindOf.CHUNG:
                case Card.KindOf.HONG:
                case Card.KindOf.CHO_B:
                    list = scores[user].Where(e => e.KindOfCard == Card.KindOf.CHO || e.KindOfCard == Card.KindOf.CHUNG || e.KindOfCard == Card.KindOf.HONG || e.KindOfCard == Card.KindOf.CHO_B).ToList();
                    start = boardPositions[user].Thee.position;
                    spacingRatio = 0.45f;
                    break;

                // [피] : 개수가 많으므로 촘촘하게 겹침 (30% 간격)
                case Card.KindOf.P:
                case Card.KindOf.PP:
                case Card.KindOf.PPP:
                    list = scores[user].Where(e => e.KindOfCard == Card.KindOf.P || e.KindOfCard == Card.KindOf.PP || e.KindOfCard == Card.KindOf.PPP).ToList();
                    start = boardPositions[user].Pee.position;
                    spacingRatio = 0.35f;
                    break;
            }

            // [핵심] 최종 목적지(end) 계산
            // 현재 쌓인 카드 개수(currentIndex)만큼 옆으로 밀어줍니다.
            int currentIndex = list.Count;

            // X축: 카드 너비 * 비율 * 개수만큼 이동
            float xOffset = card.Width * spacingRatio * currentIndex;
            float zOffset = 0f;

            // Z축: 카드가 쌓일수록 카메라 쪽으로(혹은 위로) 미세하게 올라와야 겹침 버그가 안 생김
            // 값이 -0.01f 인지 +0.01f 인지는 카메라 방향에 따라 조정하세요. (보통 -Z가 카메라 쪽)
            float yOffset = 0.02f * currentIndex;

            // 최종 좌표 설정
            // start.y는 바닥 높이 유지
            Vector3 end = new Vector3(start.x + xOffset, start.y + yOffset, start.z + zOffset);

            // 이동 애니메이션 실행
            // interval은 여러 장을 동시에 먹을 때 카드별 출발 지연 시간
            float interval = 0.5f;

            card.Owner = (Player)user;

            // 카드를 바로 scores에 넣지 않고, 애니메이션 시작과 동시에 넣습니다.
            // (그래야 다음 카드가 이 카드의 위치를 참고해서 그 옆에 붙습니다 - 연속 획득 시)
            scores[user].Add(card);

            // JumpTo (DOMove + 포물선)
            card.JumpTo(end, jumpPower: 2f, time: 0.5f, delay: count * interval, complete: complete);
        }

        /// <summary>
        /// 
        /// </summary>
        private void TackeCardToScore()
        {
            Debug.Log($"먹는 판정 패 : {listEat.Count}");
            int total = listEat.Count;
            if (total == 0)
            {
                return;
            }

            int count = 0;
            foreach (var card in listEat)
            {
                var slot = GetSlot(card);
                slot.Value.Remove(card); // 보드 슬롯에서 제거.
                count++;
            }
        }

        /// <summary>
        /// 획득 카드 체크.
        /// </summary>
        /// <returns></returns>
        private bool TakeCardCondition()
        {
            bool possibleEat = false;
            foreach (KeyValuePair<int, CardList> kindSlot in bottoms)
            {
                var list = kindSlot.Value.GetListNot13();
                var listJocker = kindSlot.Value.GetList(13);

                if (listJocker.Count > 0)
                {
                    stealCount += listJocker.Count;
                    listEat.AddRange(listJocker);
                }

                switch (list.Count)
                {
                    case 0:
                    case 1:
                        break;
                    case 2:
                        if (list[0].Owner == turnUser &&
                            list[1].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            stealCount++;

                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("귀신");
                        }
                        else if (list[0].Owner == Player.None &&
                                 list[1].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                        }
                        else
                        {
                        }
                        break;
                    case 3:
                        if (list[0].Owner == Player.None &&
                            list[1].Owner == turnUser &&
                            list[2].Owner == Player.None)
                        {
                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("뻑1.");
                        }
                        else if (list[0].Owner == Player.None &&
                                  list[1].Owner == turnUser &&
                                  list[2].Owner == turnUser)
                        {
                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("뻑2");
                        }
                        else if (list[0].Owner == Player.None &&
                                list[1].Owner == Player.None &&
                                list[2].Owner == turnUser)
                        {
                            
                            foreach (var card in list)
                            {
                                if (card.Owner == Player.None)
                                {
                                    select.Add(card);
                                }
                                else if (card.Owner == turnUser)
                                {
                                    listEat.Add(card);
                                }
                            }

                            if (select.Count == 2)
                            {
                            }

                            possibleEat = true;
                        }
                        else
                        {
                        }

                        break;
                    case 4:
                        if (list[0].Owner == Player.None &&
                            list[1].Owner == Player.None &&
                            list[2].Owner == Player.None &&
                            list[3].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            stealCount++;
                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("아싸~");
                        }
                        else if (list[0].Owner == Player.None &&
                                list[1].Owner == Player.None &&
                                list[2].Owner == turnUser &&
                                list[3].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            stealCount++;
                            select.Clear();
                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("따닥1");
                        }
                        else if (list[0].Owner == Player.None &&
                                list[1].Owner == turnUser &&
                                list[2].Owner == turnUser &&
                                list[3].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            stealCount++;
                            select.Clear();
                            var popup = UIManager.Instance.OpenPopup<UIPopupMessage>();
                            popup.Init("폭탄!!");
                        }
                        else
                        {
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }

                if (possibleEat == true)
                {
                    stealCount += listJocker.Count;
                    listEat.AddRange(listJocker);
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Card PopDeckCard()
        {
            Card card = deck.Pop();
            card.ShowMe(time: 1);
            //card.SetShadow(false);
            card.Owner = (Player)turnUser;

            hands[(int)turnUser].Add(card);
            HandSort();
            return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Card PopDeckCard(Player owner = Player.None)
        {
            if (deck.Count == 0)
                return null;

            int slotIndex = -1;
            int slotStack = 0;
            Card card = deck.Pop();

            List<Card> list = null;

            // 조커 카드.
            if (card.Month == 13 && owner != Player.None) 
            {
                foreach (var slot in bottoms)
                {
                    var count = slot.Value.Where(c => c.Owner == owner).ToList().Count();
                    if (count > 0)
                    { 
                        slotIndex = slot.Key - 1;
                        slotStack = slot.Value.Count;
                        list = slot.Value;
                        break;
                    }
                }
            }
            else 
            {
                KeyValuePair<int, CardList> slot = GetSlot(card);
                slotIndex = slot.Key-1;
                slotStack = slot.Value.Count;
                list = slot.Value;
            }

            if (slotIndex != -1)
            {
                float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
                float randZ = UnityEngine.Random.Range(-0.5f, 0.5f);
                int stackCount = slotStack;
                Vector3 destination1 = hitPosition[(int)turnUser].position;
                Vector3 destination2 = cardPosition[slotIndex].position +
                            new Vector3(randX + (stackCount * card.Width * 0.2f), stackCount * card.Height, randZ);

                card.Owner = owner;
                card.CardOpen(time: 0.1f);
                card.MoveTo(
                    destination1,
                    Vector3.one * 2,
                    time: 0.1f,
                    ease: DG.Tweening.Ease.OutCubic,
                    complete: () => {
                      card.MoveTo(
                        destination2,
                        Vector3.one,
                        time: 0.1f,
                        ease: DG.Tweening.Ease.InQuad,
                        complete: () => {
                            card.SetEnablePhysics(true);
                        });
                    });

                list.Add(card);
            }

            return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private KeyValuePair<int, CardList> GetSlot(Card card)
        {
            int key = -1;
            foreach (var kindSlot in bottoms)
            {
                var exist = kindSlot.Value.Where(e => e.Month == card.Month).FirstOrDefault();
                if (exist != null)
                {
                    key = kindSlot.Key;
                    break;
                }
            }

            KeyValuePair<int, CardList> slot;
            if (key == -1)
            {
                var emptyList = bottoms.Where(c => c.Value.Count == 0).ToList();
                slot = emptyList[UnityEngine.Random.Range(0, emptyList.Count)];
            }
            else
            {
                slot = bottoms.Where(c => c.Key == key).FirstOrDefault();
            }

            Debug.Log($"slot : {slot.Key}");
            return slot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetMoveAllCount()
        {
            int count = 0;
            foreach (var slot in bottoms)
            {
                count += slot.Value.MoveCount();// GetMoveCount();
            }

            foreach (var list in scores)
            {
                count += list.MoveCount();
            }

            return count;
        }
    }

}
