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

        /// <summary>
        /// 상대의 패를 훔칩니다.
        /// </summary>
        private void StealCard()
        {
            int target = (int)Player.None;
            
            if (turnUser == Player.Enemy)
            {
                target = (int)Player.Player;
            }
            else if (turnUser == Player.Player)
            {
                target = (int)Player.Enemy;
            }

            var listAll = scores[target].Where(e =>
                         e.KindOfCard == Card.KindOf.P ||
                         e.KindOfCard == Card.KindOf.PP ||
                         e.KindOfCard == Card.KindOf.PPP).
                         OrderBy(e => e.KindOfCard).ToList();

            var list1 = listAll.Where(e => e.KindOfCard == Card.KindOf.P).ToList();
            var list2 = listAll.Where(e => e.KindOfCard == Card.KindOf.PP).ToList();
            var list3 = listAll.Where(e => e.KindOfCard == Card.KindOf.PPP).ToList();
            if (listAll.Count == 0)
            {
                stealCount = 0;
                return;
            }

            Card card = null;
            if (stealCount >= 3 && list3.Count > 0)
            {
                card = list3[0];
            }
            else if (stealCount >= 2 && list2.Count > 0)
            {
                card = list2[0];
            }
            else
            {
                card = listAll[0];
            }

            if (listAll[0].KindOfCard == Card.KindOf.P)
                stealCount -= 1;
            else if (listAll[0].KindOfCard == Card.KindOf.PP)
                stealCount -= 2;
            else if (listAll[0].KindOfCard == Card.KindOf.PPP)
                stealCount -= 3;

            if (stealCount < 0)
                stealCount = 0;

            scores[target].Remove(card);
            listAll.Remove(card);

            TackCard(card, complete:() => {
                var start = boardPositions[target].Pee.position;
                var end = new Vector3(start.x + card.Width * 2f, start.y, start.z);

                /*
                // 기존 카드들 재배치.
                for (int i = 0; i < listAll.Count; i++)
                {
                    var c = listAll[i];
                    if (c == null)
                        continue;

                    c.SetSortOrder(i + 1);
                    c.SetEnablePhysics(true);
                    c.Owner = (Player)target;

                    Vector3 newPosition = start + new Vector3((card.Width * i) * 0.5f, 0, 0);
                    c.MoveTo(
                    newPosition,
                        time: 0.1f,
                        delay: i * 0.1f);
                }

                */
            });
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
                        var handPosition = boardPositions[i].Hand.GetChild(index).transform.position;
                        card.MoveTo(
                            handPosition,
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
                if (player == (int)Player.Player)
                {
                    enemy = (int)Player.Enemy;
                }
                else
                {
                    enemy = (int)Player.Player;
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
                    card.MoveTo(dest, time: setting.DeckCardTime, delay: i * 0.01f);
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
            for (int index = 0; index < hands[(int)Player.Player].Count; index++)
            {
                Card card = hands[(int)Player.Player][index];
                card.ShowMe(delay: index * setting.HandOpenTime);
                card.SetShadow(false);
            }

            for (int index = 0; index < hands[(int)Player.Enemy].Count; index++)
            {
                Card card = hands[(int)Player.Enemy][index];
                card.SetOpen(true);
                card.SetShadow(false);
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
                        Card card = Instantiate<Card>(prefabCard);
                        if (card != null)
                        {
                            card.Init(-1, spriteBomb);
                            card.Month = 100;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        public void HitCard(int user, Card card, float delay = 0)
        {
            var playInfo = CommandInfo.info;
     
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

                    card.SetShadow(true);
               
                    if (card.Month == 13) // 조커 카드.
                    {
                        playInfo.hited = true;
                        playInfo.hit = card;
                        stealCount += 1;

                        TackCard(card, 1); // 카드 획득.
                    }
                    else if (card.Month == 100) // 폭탄 공짜 카드.
                    {
                        playInfo.hited = true;
                        playInfo.hit = card;

                        GameObject.Destroy(card.gameObject);
                        commandProcedure.Enqueue(Command.PopCardDeck, turnUser);
                    }
                    else // 일반 카드.
                    {
                        playInfo.hit = card;
                        playInfo.hited = true;

                        slot.Value.Add(card);
                        card.MoveTo( // 카드를 위로 뽑아서.
                            destination1,
                            time: setting.HitUpTime,
                            ease: DG.Tweening.Ease.InExpo,
                            complete: () => {

                                card.MoveTo(
                                    destination2,
                                    time: setting.HitDownTime,
                                    ease: DG.Tweening.Ease.InExpo,
                                    delay: delay,
                                    complete: () =>
                                    {
                                        card.SetEnablePhysics(true);

                                        Collider[] colliders = Physics.OverlapSphere(card.transform.position, 5f);
                                        for (int i = 0; i < colliders.Length; i++)
                                        {
                                            var coll = colliders[i];
                                            if (coll == null)
                                                continue;
                                            var surroundCard = coll.GetComponent<Card>();
                                            if (surroundCard == null)
                                                continue;

                                            surroundCard.SetEnablePhysics(true);
                                            surroundCard.rigidBody.AddExplosionForce(10, card.transform.position, 10f);
                                        }

                                        Debug.Log(colliders.ToString());
                                        //card.rigidBody.AddExplosionForce(10000, card.transform.position, 10, 5f);
                                    });
                            });
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        private void TackCard(Card card, int count = 0, Action complete = null)
        {
            int user = (int)turnUser;
            List<Card> list = null;
            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            //card.gameObject.SetActive(false);
            switch (card.KindOfCard)
            {
                // 카드 두개 칸.
                case Card.KindOf.GWANG:
                case Card.KindOf.GWANG_B:
                    list = scores[user].
                        Where(e => e.KindOfCard == Card.KindOf.GWANG ||
                                   e.KindOfCard == Card.KindOf.GWANG_B).
                                    ToList();

                    start = boardPositions[user].Gwang.position;
                    //end = new Vector3(start.x + card.Width * 2f, start.y, start.z);
                    break;

                // 카드 두개 반ㅂ 칸.
                case Card.KindOf.MUNG:
                case Card.KindOf.MUNG_GODORI:
                case Card.KindOf.MUNG_KOO:
                    list = scores[user].
                        Where(e => e.KindOfCard == Card.KindOf.MUNG ||
                                   e.KindOfCard == Card.KindOf.MUNG_GODORI ||
                                   e.KindOfCard == Card.KindOf.MUNG_KOO).
                                   ToList();

                    start = boardPositions[user].Mung.position;
                    //end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
                    break;

                case Card.KindOf.CHO:
                case Card.KindOf.CHUNG:
                case Card.KindOf.HONG:
                case Card.KindOf.CHO_B:
                    list = scores[user].
                        Where(e => e.KindOfCard == Card.KindOf.CHO ||
                                   e.KindOfCard == Card.KindOf.CHUNG ||
                                   e.KindOfCard == Card.KindOf.HONG ||
                                   e.KindOfCard == Card.KindOf.CHO_B).
                                   ToList();

                    start = boardPositions[user].Thee.position;
                    //end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
                    break;

                case Card.KindOf.P:
                case Card.KindOf.PP:
                case Card.KindOf.PPP:
                    list = scores[user].
                        Where(e => e.KindOfCard == Card.KindOf.P ||
                                   e.KindOfCard == Card.KindOf.PP ||
                                   e.KindOfCard == Card.KindOf.PPP).
                                   ToList();

                    start = boardPositions[user].Pee.position;
                    //end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
                    break;
            }

            float interval = 0.05f;
            card.Owner = (Player)user;
            card.MoveTo(start, time: 0.1f,
                     delay: count * interval,
                     complete: complete);

           
            scores[(int)turnUser].Add(card);
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
            card.ShowMe();
            card.SetShadow(false);
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
                    time: 0.1f,
                    ease: DG.Tweening.Ease.OutCubic,
                    complete: () => {
                      card.MoveTo(
                        destination2,
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

            return count;
        }
    }

}
