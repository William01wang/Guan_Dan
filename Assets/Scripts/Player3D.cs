using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Rendering.Universal;
using NUnit.Framework;
using System.Linq;
using System.Runtime.InteropServices;

public class Player3D : MonoBehaviour
{
    public Quaternion direction;
    public bool operate_play = false;
    public int team = -1;
    private int card_cnt = 0;
    List<Transform> playing_cards = new List<Transform>();

    void Start()
    {
        if (gameObject.name != "Player") 
        {
            gameObject.AddComponent<Computer>();
        }
        switch (gameObject.name)
        {
            case "Player":
                team = 0;
                break;
            case "PlayerRight":
                team = 1;
                break;
            case "PlayerBack":
                team = 0;
                break;
            case "PlayerLeft":
                team = 1;
                break;
        }
    }


    public void SetDirection(Quaternion direction)
    {
        this.direction = direction;
    }

    public void AddCard(GameObject card_object, string cardname)
    {
        GameObject card = Instantiate(card_object, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        BoxCollider boxCollider = card.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.13f, 0.19f, 0.01f);
        card.AddComponent<Card3D>();
        card.GetComponent<Card3D>().cardname = $"{cardname}";
        if (gameObject.name == "Player")
        {
            card.GetComponent<Card3D>().state = Card3D.CardState.OnHand;
        }
        else
        {
            card.GetComponent<Card3D>().state = Card3D.CardState.OnOthers;
        }
        card_cnt++;
    }

    public void OrgHands()
    {
        if (card_cnt <= 0)
        {
            return;
        }
        float dx = 0.03f;
        Vector3 card_location = new Vector3(0f, 0.1f, 1.01f);
        Quaternion card_rotation = Quaternion.Euler(-20f, -1f, 0f);
        card_location.x -= (card_cnt - 1) * dx / 2;
        foreach (Transform child in transform)
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script == null)
            {
                continue;
            }
            if (card_script.state == Card3D.CardState.Selected)
            {
                card_script.SelectedToOnHand();
            }
            if (card_script.state == Card3D.CardState.OnHand || card_script.state == Card3D.CardState.OnOthers)
            {
                child.position = direction * card_location;
                child.rotation = direction * card_rotation;
                card_location.x += dx;
            }
        }
    }

    public void OrgPlayingCards()// 用于显示playing_cards变量中的牌
    {
        if (playing_cards.Count <= 0 || gameObject.name.Equals("Player"))
        {
            return;
        }
        float dx = 0.03f;
        Vector3 card_location = new Vector3(0f, 0.1f, -10f);
        switch (gameObject.name)
        {
            case "PlayerRight":
                card_location = new Vector3(-0.750f, 0.450f, 0.4f);
                break;
            case "PlayerBack":
                card_location = new Vector3(0f, 0.600f, 0.4f);
                break;
            case "PlayerLeft":
                card_location = new Vector3(0.750f, 0.450f, 0.4f);
                break;
        }
        Quaternion card_rotation = Quaternion.Euler(-20f, 0f, 0f);
        card_location.x -= (playing_cards.Count - 1) * dx / 2;
        foreach (Transform child in playing_cards)
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script == null)
            {
                continue;
            }
            child.position = card_location;
            child.rotation = card_rotation;
            card_location.x += dx;
            card_location.z -= 0.0001f;
        }
    }

    public void ResetPlayingCards()// 用于重置playing_cards变量，并销毁其中的克隆牌
    {
        for (int i = 0; i < playing_cards.Count; i++)
        {
            Destroy(playing_cards[i].gameObject);
            
        }
        playing_cards.Clear();
        playing_cards = new List<Transform>();
    }

    public void ResetHands()// 用于在新的一局开始时重置玩家在上局持有过的牌
    {
        List<Transform> lst = new List<Transform>();
        foreach (Transform child in transform)
        {
            lst.Add(child);
        }
        for (int i = 0; i < lst.Count; i++)
        {
            Destroy(lst[i].gameObject);
        }
        card_cnt = 0;
    }

    private bool PlayCardAction(Transform child)
    {
        Card3D card_script = child.GetComponent<Card3D>();
        if (card_script != null && (card_script.state == Card3D.CardState.Selected || card_script.state == Card3D.CardState.OnOthers))
        {
            card_script.state = Card3D.CardState.OnTable;
            float angle = Random.Range(-5f, 5f);
            card_script.ontable_v = direction * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, -Mathf.Cos(angle * Mathf.Deg2Rad));
            card_script.ontable_v = card_script.ontable_v.Value * Random.Range(1f, 1.2f);
            child.rotation = direction * Quaternion.Euler(-90f, -angle, 0f);
            child.position = new Vector3(Mathf.Clamp(child.position.x, -0.87f, 0.87f), GetComponentInParent<CardSet3D>().card_ontable_height, Mathf.Clamp(child.position.z, -0.87f, 0.87f));
            GetComponentInParent<CardSet3D>().card_ontable_height += 0.0001f;
            Transform temp = Instantiate(child, new Vector3(0, 0, -20), Quaternion.identity);
            temp.GetComponent<Card3D>().state = Card3D.CardState.CloneOnShowing;
            playing_cards.Add(temp);//playing_cards在一张牌打出时获得它的复制
            return true;
        }
        return false;
    }


    private void PlayCards()
    {
        if ((gameObject.name == "Player" && GetComponentInParent<CardSet3D>().whos_turn != 0) ||
            (gameObject.name == "PlayerRight" && GetComponentInParent<CardSet3D>().whos_turn != 1) ||
            (gameObject.name == "PlayerBack" && GetComponentInParent<CardSet3D>().whos_turn != 2) ||
            (gameObject.name == "PlayerLeft" && GetComponentInParent<CardSet3D>().whos_turn != 3))
        {
            return;
        }

        int card_cnt_before = card_cnt;
        if (card_cnt <= 0)
        {
            GetComponentInParent<CardSet3D>().NextTurn();
            return;
        }
        if (gameObject.name == "Player")
        {
            if (GetComponentInParent<CardSet3D>().playerUIPanel.btnPassClicked)
            {
                GetComponentInParent<CardSet3D>().playerUIPanel.btnPassClicked = false;
            }
            else if (!operate_play)
            {
                return;
            }
            else 
            {
                operate_play = false;
                foreach (Transform child in transform)
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                    }
                }
            }
            
        }
        else
        {
            string output = "";
            output = gameObject.GetComponent<Computer>().findPlayableCards(GetComponentInParent<CardSet3D>(), GetComponentInParent<CardSet3D>().cur_rank);

            if (!output.Equals("")) 
            {
                string[] output_split = output.Split('-');
                int[] output_split_int = new int[output_split.Length];
                for (int i = 0; i < output_split.Length; i++)
                {
                    output_split_int[i] = int.Parse(output_split[i]);
                }
                switch (output_split_int[0]) 
                {
                    case 1:
                        UpdateCardStatus(output_split_int[1]);
                        break;
                    case 2:
                        for (int i = 0; i < 2; i++) 
                        {
                            UpdateCardStatus(output_split_int[1]);
                        }
                        break;
                    case 3:
                        for (int i = 0; i < 3; i++)
                       {
                            UpdateCardStatus(output_split_int[1]);
                        }
                        break;
                    case 4:
                       for (int i = 0; i < 5; i++)
                        {
                            UpdateCardStatus(output_split_int[1]+i);
                        }
                        break;
                    case 5:
                        for (int i = 0; i < 3; i++)
                        {
                            UpdateCardStatus(output_split_int[1]);
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            UpdateCardStatus(output_split_int[2]);
                        }
                        break;
                    case 6:
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                UpdateCardStatus(output_split_int[1] + i);
                            }
                        }
                        break;
                    case 7:
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                UpdateCardStatus(output_split_int[1] + i);
                            }
                        }
                        break;
                    case 8:
                        for (int i = 0; i < output_split_int[1]; i++)
                        {
                            UpdateCardStatus(output_split_int[2]);
                        }
                        break;
                
                }
            }
            GetComponentInParent<CardSet3D>().UpdateCardOnDeck(output, gameObject.name);

        }

        if (card_cnt <= 0)
        {
            GetComponentInParent<CardSet3D>().n_finished_player++;
            if (GetComponentInParent<CardSet3D>().n_finished_player == 1)
            {
                GetComponentInParent<CardSet3D>().top_team = team;
            }
            else if (GetComponentInParent<CardSet3D>().n_finished_player > 1 && GetComponentInParent<CardSet3D>().top_team == team) 
            {
                GetComponentInParent<CardSet3D>().rank_up += (4 - GetComponentInParent<CardSet3D>().n_finished_player);
            }

        }
        OrgHands();
        OrgPlayingCards();
        GetComponentInParent<CardSet3D>().NextTurn();
    }

    public string CheckHandType()
    {
        int cur_rank = GetComponentInParent<CardSet3D>().cur_rank;
        List<string> cardsOnPlaying = new List<string>();
        foreach (Transform child in transform)//获取所有被选中的牌，牌在cardsOnplaying从小到大排序
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script != null && card_script.state == Card3D.CardState.Selected)
            {
                cardsOnPlaying.Add(card_script.cardname);
            }
        }
        //cardsOnPlaying.ForEach(x => Debug.Log(x));  //用于debug查看试图打出的牌
        string[] suits = new string[cardsOnPlaying.Count];
        int[] points = new int[cardsOnPlaying.Count];
        int num_of_normal = 0;
        int num_of_heart_rank = 0;
        int heart_rank_used = 0;
        for (int i = 0; i < cardsOnPlaying.Count; i++) //对选中牌的花色和大小进行标准化
        {
            string[] cur = cardsOnPlaying[i].Split("-");
            suits[num_of_normal] = cur[0];
            if (cur[1] == "Jack")
            {
                points[num_of_normal] = 11;
            }
            else if (cur[1] == "Queen")
            {
                points[num_of_normal] = 12;
            }
            else if (cur[1] == "King")
            {
                points[num_of_normal] = 13;
            }
            else if (cur[1] == "Ace")
            {
                points[num_of_normal] = 14;
            }
            else if (cur[1] == "LittleJoker")
            {
                points[num_of_normal] = 16;
            }
            else if (cur[1] == "BigJoker")
            {
                points[num_of_normal] = 17;
            }
            else
            {
                points[num_of_normal] = int.Parse(cur[1]);
            }
            if (suits[num_of_normal].Equals("Heart") && points[num_of_normal] == cur_rank) 
            {
                num_of_heart_rank++;
            }
            else
            {
                num_of_normal++;
            }
        }

        //开始判断牌型
        if (cardsOnPlaying.Count == 1) //单牌
        {
            if (num_of_normal == 0) 
            {
                return $"1-{cur_rank}";
            }
            return $"1-{points[0]}";
        }
        else if (cardsOnPlaying.Count == 2) //对子
        {
            if (num_of_normal == 0)
            {
                return $"1-{cur_rank}";
            }
            else if (num_of_normal == 1)
            {
                return $"2-{points[0]}";
            }
            else 
            {
                if (points[0] == points[1]) 
                {
                    return $"2-{points[0]}";
                }
            }

        }
        else if (cardsOnPlaying.Count == 3) //三同张
        {
            switch (num_of_normal) 
            {
                case 0:
                    return $"3-{cur_rank}";
                case 1:
                    return $"3-{points[0]}";
                case 2:
                    if (points[0] == points[1])
                    {
                        return $"3-{points[0]}";
                    }
                    break;
                case 3:
                    if (points[0] == points[1] && points[0] == points[2])
                    {
                        return $"3-{points[0]}";
                    }
                    break;
            }
            
        }
        else if (cardsOnPlaying.Count == 5) //顺子、同花顺和三带对
        {
            bool flush = true;
            bool Straight = true;
            for (int i = 0; i < num_of_normal - 1; i++) //判断是否同花
            {
                if (!suits[i].Equals(suits[i + 1]))
                {
                    flush = false; break;
                }
            }
            for (int i = 0; i < num_of_normal - 1; i++)//判断是否顺子
            {
                if (points[i] != points[i + 1] - 1)
                {
                    for (int j = 0; j < points[i + 1] - points[i] - 1; j++) 
                    {
                        if (num_of_heart_rank > heart_rank_used)
                        {
                            heart_rank_used++;
                        }
                        else 
                        {
                            heart_rank_used = 0;
                            Straight = false; break;
                        }
                    }
                    if (!Straight) 
                    {
                        break;
                    }
                }
            }
            if (!Straight && points[num_of_normal - 1] == 14) //单独考虑A2345情况
            {
                Straight = true;
                if (1 != points[0] - 1) 
                {
                    for (int j = 0; j < points[0] - 2; j++)
                    {
                        if (num_of_heart_rank > heart_rank_used)
                        {
                            heart_rank_used++;
                        }
                        else
                        {
                            heart_rank_used = 0;
                            Straight = false; break;
                        }
                    }
                }
                for (int i = 0; i < num_of_normal - 2; i++)
                {
                    if (!Straight)
                    {
                        break;
                    }
                    if (points[i] != points[i + 1] - 1)
                    {
                        for (int j = 0; j < points[i + 1] - points[i] - 1; j++)
                        {
                            if (num_of_heart_rank > heart_rank_used)
                            {
                                heart_rank_used++;
                            }
                            else
                            {
                                heart_rank_used = 0;
                                Straight = false; break;
                            }
                        }
                    }
                }
                if (flush && Straight)
                {
                    return $"9-{1}";
                }
                else if (Straight)
                {
                    return $"4-{1}";
                }
            }
            if (flush && Straight)
            {
                return $"9-{points[0]}";
            }
            else if (Straight)
            {
                return $"4-{points[0]}";
            }
            int point1 = points[0], point2 = -1, count1 = 1, count2 = 0;
            for (int i = 1; i < num_of_normal; i++) //判断是否三带对
            {
                if (points[i] == point1)
                {
                    count1++;
                }
                else if (point2 == -1)
                {
                    point2 = points[i];
                    count2++;
                }
                else if (points[i] == point2)
                {
                    count2++;
                }
                else
                {
                    break;
                }
            }
            if (count1 + count2 == num_of_normal)
            {
                if (count1 > 3 || count2 > 3){}
                else if (count1 == 3)
                {
                    return $"5-{point1}-{point2}";
                }
                else
                {
                    return $"5-{point2}-{point1}";
                }
            }
        }
        else if (cardsOnPlaying.Count == 6) //三连对和三同连张
        {
            int point1 = points[0], point2 = -1, point3 = -1, count1 = 1, count2 = 0, count3 = 0;
            for (int i = 1; i < num_of_normal; i++)
            {
                if (points[i] == point1)
                {
                    count1++;
                }
                else if (point2 == -1)
                {
                    point2 = points[i];
                    count2++;
                }
                else if (points[i] == point2)
                {
                    count2++;
                }
                else if (point3 == -1)
                {
                    point3 = points[i];
                    count3++;
                }
                else if (points[i] == point3)
                {
                    count3++;
                }
                else
                {
                    break;
                }
            }
            if (count1 + count2 + count3 == num_of_normal) 
            {
                if (count3 == 0)
                {
                    if (count1 > 3 || count2 > 3 || (point2 - point1 > 1 && point2 - point1 != 12)) { }
                    else if (count1 == 3 || count2 == 3)
                    {
                        return $"7-{point1}";
                    }
                    else //这种情况下，只可能是2-2普通配两张赖子，使得可能出现三连对和三同连张两种情况
                    {
                        string best_value = GetComponentInParent<CardSet3D>().GetBestCardOnDeck();
                        if (best_value == "") //本轮牌桌上没牌，则两种牌型应当可以自由选择一种，但是还需要在UI上实现，很麻烦，且这种情况比较极端，所以直接视作打出相对更稀有的三同连张
                        {
                            return $"7-{point1}";
                        }
                        string[] best_value_split = GetComponentInParent<CardSet3D>().GetBestCardOnDeck().Split('-');
                        if (int.Parse(best_value_split[0]) == 6) //牌桌上有连对，则只能是跟连对
                        {
                            return $"6-{point1}";
                        }
                        else //牌桌上是三同连对，则只能是跟三同连对，如果两种都不是，反正会在后面的检测被拦截，所以返回哪种都无所谓
                        {
                            return $"7-{point1}";
                        }
                    }
                }
                else 
                {
                    if (count1 > 2 || count2 > 2 || count3 > 2 || point2 - point1 > 1 || (point3 - point2 > 1 && point3 - point1 != 12)) {}
                    else 
                    {
                        return $"6-{point1}";
                    }
                }
            }
        }

        bool Bomb = true;
        for (int i = 0; i < num_of_normal - 1; i++) //判断炸弹
        {
            if (points[i] != points[i + 1])
            {
                Bomb = false; break;
            }
        }
        if (Bomb)
        {
            return $"8-{cardsOnPlaying.Count}-{points[0]}";
        }

        if (cardsOnPlaying.Count == 4 && num_of_heart_rank == 0) //判断四大天王
        {
            Bomb = true;
            for (int i = 0; i < cardsOnPlaying.Count - 1; i++)
            {
                if (points[i] < 16)
                {
                    Bomb = false; break;
                }
            }
            if (Bomb)
            {
                return $"8-11-16";
            }
        }
        return "";
    }
    //牌型大小的设计：
    //使用string
    //单牌："1-牌点"
    //对子："2-牌点"
    //三同张："3-牌点"
    //顺子："4-最小牌牌点"
    //三带对："5-三的牌点-二的牌点"
    //三连对："6-最小牌牌点"
    //三同连张："7-最小牌牌点"
    //炸弹："8-炸弹长度-牌点" 其中四大天王长度视为11，牌点设为16
    //同花顺："9-牌点"


    //用于处理电脑玩家打出牌的动画
    void UpdateCardStatus(int point) 
    {
        if (point <= 10)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"{point}"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 11) 
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"Jack"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 12)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"Queen"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 13)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"King"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 14)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"Ace"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 16)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"LittleJoker"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
        else if (point == 17)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Regex.IsMatch(child.GetComponent<Card3D>().cardname, $"BigJoker"))
                {
                    if (PlayCardAction(child))
                    {
                        card_cnt--;
                        break;
                    }
                }
            }
        }
    }


    void Update()
    {
        PlayCards();
    }


}




