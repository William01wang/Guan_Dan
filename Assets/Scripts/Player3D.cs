using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Rendering.Universal;

public class Player3D : MonoBehaviour
{
    public Quaternion direction;
    public bool operate_play = false;
    private int card_cnt = 0;

    void Start()
    {
        if (gameObject.name != "Player") 
        {
            gameObject.AddComponent<Computer>();
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
            return true;
        }
        return false;
    }

    private void NextTrun()
    {
        GetComponentInParent<CardSet3D>().whos_turn++;
        switch (GetComponentInParent<CardSet3D>().whos_turn)
        {
            case 0:
                GetComponentInParent<CardSet3D>().UpdateCardOnDeck("", "Player");
                Debug.Log("Player's turn!");
                break;
            case 1:
                GetComponentInParent<CardSet3D>().UpdateCardOnDeck("", "PlayerRight");
                Debug.Log("PlayerRight's turn!");
                break;
            case 2:
                GetComponentInParent<CardSet3D>().UpdateCardOnDeck("", "PlayerBack");
                Debug.Log("PlayerBack's turn!");
                break;
            case 3:
                GetComponentInParent<CardSet3D>().UpdateCardOnDeck("", "PlayerLeft");
                Debug.Log("PlayerLeft's turn!");
                break;
            default:
                GetComponentInParent<CardSet3D>().whos_turn = 0;
                GetComponentInParent<CardSet3D>().UpdateCardOnDeck("", "Player");
                Debug.Log("Player's turn!");
                GetComponentInParent<CardSet3D>().playerUIPanel.setPanelActive(true);
                foreach (string v in GetComponentInParent<CardSet3D>().card_on_deck.Values)
                {
                    Debug.Log(v);
                }
                break;
        }
    }

    private void PlayCards()
    {
        int card_cnt_before = card_cnt;
        if ((gameObject.name == "Player" && GetComponentInParent<CardSet3D>().whos_turn != 0) ||
            (gameObject.name == "PlayerRight" && GetComponentInParent<CardSet3D>().whos_turn != 1) ||
            (gameObject.name == "PlayerBack" && GetComponentInParent<CardSet3D>().whos_turn != 2) ||
            (gameObject.name == "PlayerLeft" && GetComponentInParent<CardSet3D>().whos_turn != 3))
        {
            return;
        }
        if (card_cnt <= 0)
        {
            NextTrun();
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
            output = gameObject.GetComponent<Computer>().findPlayableCards(GetComponentInParent<CardSet3D>());

            if (!output.Equals("")) 
        {
                int[] output_split_int = new int[3];
            string[] output_split = output.Split('-');
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
                        for (int j = 0; j < 2; i++)
                        {
                            UpdateCardStatus(output_split_int[1] + i);
                        }
                    }
                    break;
                case 7:
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 3; i++)
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
            GetComponentInParent<CardSet3D>().player_rank[gameObject.name] = GetComponentInParent<CardSet3D>().n_finished_player;
        }
        OrgHands();
        NextTrun();
    }

    public string CheckHandType()
    {
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
        for (int i = 0; i < cardsOnPlaying.Count; i++) //对选中牌的花色和大小进行标准化
        {
            string[] cur = cardsOnPlaying[i].Split("-");
            suits[i] = cur[0];
            if (cur[1] == "Jack")
            {
                points[i] = 11;
            }
            else if (cur[1] == "Queen")
            {
                points[i] = 12;
            }
            else if (cur[1] == "King")
            {
                points[i] = 13;
            }
            else if (cur[1] == "Ace")
            {
                points[i] = 14;
            }
            else if (cur[1] == "LittleJoker")
            {
                points[i] = 16;
            }
            else if (cur[1] == "BigJoker")
            {
                points[i] = 17;
            }
            else
            {
                points[i] = int.Parse(cur[1]);
            }
        }
        //开始判断牌型
        if (cardsOnPlaying.Count == 1) //单牌
        {
            return $"1-{points[0]}";
        }
        else if (cardsOnPlaying.Count == 2) //对子
        {
            if (points[0] == points[1])
            {
                return $"2-{points[0]}";
            }
        }
        else if (cardsOnPlaying.Count == 3) //三同张
        {
            if (points[0] == points[1] && points[0] == points[2])
            {
                return $"3-{points[0]}";
            }
        }
        else if (cardsOnPlaying.Count == 5) //顺子、同花顺和三带对
        {
            bool flush = true;
            bool Straight = true;
            for (int i = 0; i < cardsOnPlaying.Count - 1; i++) //判断是否同花
            {
                if (!suits[i].Equals(suits[i + 1]))
                {
                    flush = false; break;
                }
            }
            for (int i = 0; i < cardsOnPlaying.Count - 1; i++)//判断是否顺子
            {
                if (points[i] != points[i + 1] - 1)
                {
                    Straight = false; break;
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
            for (int i = 1; i < cardsOnPlaying.Count; i++) //判断是否三带对
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
            if (count1 + count2 == 5)
            {
                if (count1 == 3)
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
            for (int i = 1; i < cardsOnPlaying.Count; i++)
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
            if ((count1 == 3) && (count2 == 3)) //判断是否三同连张
            {
                return $"7-{point1}";
            }
            else if ((count1 == 2) && (count2 == 2) && (count3 == 2)) //判断是否三连对
            {
                return $"6-{point1}";
            }
        }
        bool Bomb = true;
        for (int i = 0; i < cardsOnPlaying.Count - 1; i++) //判断炸弹
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

        if (cardsOnPlaying.Count == 4) //判断四大天王
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




