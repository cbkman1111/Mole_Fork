using DG.Tweening;
using System;

using System.Collections.Generic;
using System.Linq;
using Common.Global;
using UI.Menu;
using UI.Popup;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;
using UnityEditor;


namespace Gostop
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public Dictionary<int, List<Card>> bottoms = null;
        public List<Card>[] hands = null;
        public List<Card>[] scores = null;
        public Stack<Card> deck = null;
        private List<Card> select = null; // 선택해야 하는 카드.
        private List<Card> listEat = null; // 먹는패.

        /// <summary>
        /// 상대의 패를 훔칩니다.
        /// </summary>
        private void StealCard()
        {
            int target = (int)Player.NONE;
            
            if (turnUser == Player.COMPUTER)
            {
                target = (int)Player.USER;
            }
            else if (turnUser == Player.USER)
            {
                target = (int)Player.COMPUTER;
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

            EatCard(card, complete:() => {
                var start = boardPositions[target].Pee.position;
                var end = new Vector3(start.x + card.Width * 2f, start.y, start.z);

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
                GameObject.Destroy(deck.Pop().gameObject);
            }

            foreach (var kindSlot in bottoms)
            {
                foreach (var card in kindSlot.Value)
                {
                    GameObject.Destroy(card.gameObject);
                }
            }

            for (int i = 0; i < (int)Player.MAX; i++)
            {
                foreach (var card in hands[i])
                {
                    GameObject.Destroy(card.gameObject);
                }
            }

            for (int i = 0; i < (int)Player.MAX; i++)
            {
                foreach (var card in scores[i])
                {
                    GameObject.Destroy(card.gameObject);
                }
            }

            // 배열 지우기.
            deck.Clear();
            stateMachine.Clear();

            foreach (var kindSlot in bottoms)
            {
                kindSlot.Value.Clear();
            }

            for (int i = 0; i < 2; i++)
            {
                hands[i].Clear();
            }

            for (int i = 0; i < (int)Player.MAX; i++)
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
        public bool SortHand()
        {
            for (int i = 0; i < (int)Player.MAX; i++)
            {
                hands[i] = hands[i].OrderBy(card => card.Num).ToList();

                for (int index = 0; index < hands[i].Count; index++)
                {
                    var card = hands[i][index];
                    var handPosition = boardPositions[i].Hand.GetChild(index).transform.position;
                    //var handRotation = boardPositions[i].Hand.transform.rotation;

                    card.MoveTo(
                        handPosition,
                        time: 0.2f);
                }
              
            }
            return true;
        }

        /// <summary>
        /// 스코어 처리.
        /// </summary>
        private void ScoreUpdate()
        {
            for (int user = 0; user < (int)Player.MAX; user++)
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
                var list = scores[user].Where(card => card.KindOfCard == Card.KindOf.P || card.KindOfCard == Card.KindOf.PP || card.KindOfCard == Card.KindOf.PPP).ToList();
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
            for (int user = 0; user < (int)Player.MAX; user++)
            {
                int player = (int)user;
                int enemy = (int)Player.COMPUTER;
                if (player == (int)Player.USER)
                {
                    enemy = (int)Player.COMPUTER;
                }
                else
                {
                    enemy = (int)Player.USER;
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

                menu.ScoreUpdate((Player)user, gameScore[user]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CreateDeck()
        {
            List<int> nums = new List<int>();
            int cardCount = 48;
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
                    card.transform.position = deckPosition.position;
                    card.MoveTo(new Vector3(deckPosition.position.x, height * i, deckPosition.position.z), delay: i * 0.01f);
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

                KeyValuePair<int, List<Card>> slot = GetSlot(card);
                float randX = UnityEngine.Random.Range(0.2f, 0.3f);
                float randZ = UnityEngine.Random.Range(0.1f, 0.15f);

                var stack = slot.Value.Count;
                var y = card.Height * stack;
                Vector3 position = cardPosition[slot.Key - 1].position + new Vector3(i * randX, y, i * randZ);
  
                card.MoveTo(
                    position,
                    time: 0.08f,
                    delay: i * 0.025f, 
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
                        time: 0.1f,
                        delay: user * 0.2f + i * 0.1f);

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
                var list = slot.Value.Where(e => e.Month == 13).ToList();
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    var card = list[i];
                    EatCard(card, list.Count - i);
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
                foreach (var card in slot.Value)
                {
                    card.CardOpen(0.2f);
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
                    //card.SetPhysicDiable(true);
                    card.CardOpen(0.1f);
                    card.MoveTo(
                        slot.position,
                        time: 0.1f,
                        delay: i * 0.05f);
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
            for (int index = 0; index < hands[0].Count; index++)
            {
                Card card = hands[0][index];
                card.ShowMe(delay: index * 0.05f);
                card.SetShadow(false);
            }

            for (int index = 0; index < hands[1].Count; index++)
            {
                Card card = hands[1][index];
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
            return hands[user].Where(c => c.Month == card.Month && c.Month != 13).ToList();
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

            foreach (KeyValuePair<int, List<Card>> kindSlot in bottoms)
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
            var playInfo = StateInfo.info;
     
            KeyValuePair<int, List<Card>> slot = GetSlot(card);
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
                        stealCount += 1;
                        EatCard(card);
                        stateMachine.Change(State.STEAL, turnUser);
                        stateMachine.Change(State.CARD_POP_AND_HIT, turnUser);
                        StateInfo = null;
                        /*
                        var deckCard = deck.Pop();
                        deckCard.MoveTo(card.transform.position, time: 0.1f);
                        deckCard.ShowMe();
                        deckCard.SetShadow(false);
                        deckCard.Owner = (Player)user;
                        hands[user].Add(deckCard);
                        */
                    }
                    else if (card.Month == 100) // 폭탄 공짜 카드.
                    {
                        GameObject.Destroy(card.gameObject);
                        //stateMachine.Change(State.CARD_POP_AND_HIT, turnUser);
                        StateInfo = null;
                    }
                    else // 일반 카드.
                    {
                        playInfo.hit = card;
                        playInfo.hited = true;

                        slot.Value.Add(card);
                        card.MoveTo( // 카드를 위로 뽑아서.
                            destination1,
                            time: 0.2f,
                            ease: DG.Tweening.Ease.InExpo,
                            complete: () => {

                                card.MoveTo(
                                    destination2,
                                    time: 0.2f,
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
        private void EatCard(Card card, int count = 0, Action complete = null)
        {
            int user = (int)turnUser;
            List<Card> list = null;
            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            card.gameObject.SetActive(false);
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
                    end = new Vector3(start.x + card.Width * 2f, start.y, start.z);
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
                    end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
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
                    end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
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
                    end = new Vector3(start.x + card.Width * 2.5f, start.y, start.z);
                    break;
            }

            float interval = 0.05f;

            // 기존 카드들 재배치.
            for (int i = 0; i < list.Count; i++)
            {
                var c = list[i];
                if (c == null) 
                    continue;

                c.SetSortOrder(i + 1);
                c.SetEnablePhysics(true);
                c.Owner = (Player)user;
                
                Vector3 newPosition = start + new Vector3((card.Width * i) * 0.5f, 0, 0);
                c.MoveTo(
                    newPosition,
                    time: 0.1f,
                    delay: count * interval);
            }

            // 카드 가져오기.
            var position1 = new Vector3(card.transform.position.x, 1, card.transform.position.z);
            Vector3 targetPosition = start + new Vector3((card.Width * list.Count) * 0.5f, 0, 0);
            // 위로 살짝뛰웠다가.
            card.Owner = (Player)user;
            card.MoveTo(
                position1,
                time: 0.1f,
                delay: count * interval,
                complete: () => {
                        // 목적 위치로.
                        card.MoveTo(
                            targetPosition,
                            time: 0.1f,
                            delay: count * interval,
                            complete: () => {

                                complete();
                            });
                });

            scores[(int)turnUser].Add(card);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Eat()
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
                EatCard(card, total - count); // 카드 획득.
                var slot = GetSlot(card);
                slot.Value.Remove(card); // 보드 슬롯에서 제거.
                count++;
            }


            listEat.Clear();
        }

        /// <summary>
        /// 획득 카드 체크.
        /// </summary>
        /// <returns></returns>
        private bool EatCheck()
        {
            bool possibleEat = false;
            foreach (KeyValuePair<int, List<Card>> kindSlot in bottoms)
            {
                var list = kindSlot.Value.Where(c => c.Month != 13).ToList();
                var listJocker = kindSlot.Value.Where(c => c.Month == 13).ToList();

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
                            DebugLog($"{list[0].Month} - 귀신.");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                 list[1].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            DebugLog($"{list[0].Month} - 두장.");
                        }
                        else
                        {
                            DebugLog($"{list[0].Month} - 두장. ??");
                        }
                        break;
                    case 3:
                        if (list[0].Owner == Player.NONE &&
                            list[1].Owner == turnUser &&
                            list[2].Owner == Player.NONE)
                        {
                            DebugLog($"{list[0].Month} - 쌋다. 1");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                  list[1].Owner == turnUser &&
                                  list[2].Owner == turnUser)
                        {
                            DebugLog($"{list[0].Month} - 쌋다. 2");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                list[1].Owner == Player.NONE &&
                                list[2].Owner == turnUser)
                        {
                            
                            foreach (var card in list)
                            {
                                if (card.Owner == Player.NONE)
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
                                DebugLog($"{list[0].Month} - 골르기. {select.Count}");
                            }


                            possibleEat = true;
                            
                        }
                        else
                        {
                            DebugLog($"{list[0].Month} - 3장. ??");
                        }

                        break;
                    case 4:
                        if (list[0].Owner == Player.NONE &&
                            list[1].Owner == Player.NONE &&
                            list[2].Owner == Player.NONE &&
                            list[3].Owner == turnUser)
                        {
                            foreach (var card in list)
                            {
                                listEat.Add(card);
                            }

                            possibleEat = true;
                            stealCount++;
                            DebugLog($"{list[0].Month} - 아싸.");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                list[1].Owner == Player.NONE &&
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
                            DebugLog($"{list[0].Month} - 따닥. 1");
                        }
                        else if (list[0].Owner == Player.NONE &&
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
                            DebugLog($"{list[0].Month} - 따닥. 2");
                        }
                        else
                        {
                            DebugLog($"{list[0].Month} - 4장. ??");
                        }
                        break;
                    default:
                        {
                            //DebugLog($"{list[0].Month} - 5장. ??");
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

            var deckCard = deck.Pop();
            deckCard.MoveTo(card.transform.position, time: 0.1f);
            deckCard.ShowMe();
            deckCard.SetShadow(false);
            deckCard.Owner = (Player)turnUser;
            hands[(int)turnUser].Add(deckCard);
            
            SortHand();
            return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Card Pop1Cards(Player owner = Player.NONE)
        {
            Card card = deck.Pop();
            KeyValuePair<int, List<Card>> slot = GetSlot(card);
            if (slot.Key != -1)
            {
                float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
                float randZ = UnityEngine.Random.Range(-0.5f, 0.5f);
                int stackCount = slot.Value.Count;
                Vector3 destination1 = hitPosition[(int)turnUser].position;
                Vector3 destination2 = cardPosition[slot.Key - 1].position +
                            new Vector3(randX, stackCount * card.Height, randZ);

                card.CardOpen(time: 0.2f);
                card.Owner = owner;
                card.MoveTo(
                    destination1,
                    time: 0.2f,
                    ease: DG.Tweening.Ease.OutCubic,
                    complete: () => {
                      card.MoveTo(
                        destination2,
                        time: 0.2f,
                        ease: DG.Tweening.Ease.InQuad,
                        complete: () => {
                            card.SetEnablePhysics(true);
                        });
                    });

                slot.Value.Add(card);
            }

            return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private KeyValuePair<int, List<Card>> GetSlot(Card card)
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

            KeyValuePair<int, List<Card>> slot;
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
                count += GetMoveCount(slot.Value);
            }

            return count;
        }
    }

}
