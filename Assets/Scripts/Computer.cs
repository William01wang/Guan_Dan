using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

//一个用于电脑玩家的类
//目前简单人机的思路是在游戏开始时把拥有手牌遍历一次，生成手牌库。之后每次出牌只在可打牌型中选择可打的最小牌。遍历手牌库时生成一个(牌点，数量)的字典，代表
public class Computer : MonoBehaviour
{
    Dictionary<int, int> handCards = new Dictionary<int, int>();//手牌

    void Start()
    {
    }
    public void BuildHandCards(Player3D player) //生成手牌
    {
        for (int i = 1; i <= 17; i++) //初始化
        {
            handCards.Add(i, 0);
        }
        List<string> cards = new List<string>();
        foreach (Transform child in player.transform)//获取所有手牌，从小到大排序
        {
            Card3D card_script = child.GetComponent<Card3D>();
            cards.Add(card_script.cardname);
        }
        for (int i = 0; i < cards.Count; i++) //对选中牌的花色和大小进行标准化
        {
            string[] cur = cards[i].Split("-");
            if (cur[1] == "Jack")
            {
                handCards[11]++;
            }
            else if (cur[1] == "Queen")
            {
                handCards[12]++;
            }
            else if (cur[1] == "King")
            {
                handCards[13]++;
            }
            else if (cur[1] == "Ace")
            {
                handCards[14]++;
            }
            else if (cur[1] == "LittleJoker")
            {
                handCards[16]++;
            }
            else if (cur[1] == "BigJoker")
            {
                handCards[17]++;
            }
            else
            {
                handCards[int.Parse(cur[1])]++;
            }
        }
    }

    public string findPlayableCards(Dictionary<string, string> card_on_deck) //根据本轮牌桌上的牌，寻找可打出的最小牌型
    {
        string result = "";
        string best_value = CardSet3D.GetBestCardOnDeck(card_on_deck);
        if (best_value.Equals("")) //若没人出过牌，直接打最小的牌型
        {
            if (result == "") 
            {
                result = Search_Triple_With_Row(0);
            }
            if (result == "")
            {
                result = Search_Three_pairs(0);
            }
            if (result == "")
            {
                result = Search_Three_pairs(0);
            }
            if (result == "")
            {
                result = Search_Triple_Pairs(0);
            }
            if (result == "")
            {
                result = Search_Straight(0);
            }
            if (result == "")
            {
                result = Search_Double(0);
            }
            if (result == "")
            {
                result = Search_Single(0);
            }
        }
            else
            {
                int[] best_value_int = new int[3];
                string[] best_value_split = best_value.Split('-');
                for (int i = 0; i < best_value_split.Length; i++)
                {
                    best_value_int[i] = int.Parse(best_value_split[i]);
                }
                if (best_value_int[0] == 8) //牌桌上最大为炸弹
                {
                    return Search_Bomb(best_value_int[1], best_value_int[2]);
                }
                else if (best_value_int[0] == 9) //牌桌上最大为同花顺
                {
                    return Search_Bomb(6, 0);
                }
                else //牌桌上最大为普通牌型
                {
                    switch (best_value_int[0]) //查找同牌型
                    {
                        case 1:
                            result = Search_Single(best_value_int[1]);
                            break;
                        case 2:
                            result = Search_Double(best_value_int[1]);
                            break;
                        case 3:
                            result = Search_Triple(best_value_int[1]);
                            break;
                        case 4:
                            result = Search_Straight(best_value_int[1]);
                            break;
                        case 5:
                            result = Search_Triple_Pairs(best_value_int[1]);
                            break;
                        case 6:
                            result = Search_Three_pairs(best_value_int[1]);
                            break;
                        case 7:
                            result = Search_Triple_With_Row(best_value_int[1]);
                            break;
                    }
                    if (result.Equals("")) //若同牌型要不起，找炸弹
                    {
                        result = Search_Bomb(4, 0);
                    }
                   
                }
            }
        return result;
    }

    string Search_Single(int point) //查找可出最小单牌
    {
        for (int i = point+1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= 1) 
            {
                handCards[i]--;
                return $"1-{i}";
            }
        }
        return "";
    }

    string Search_Double(int point) //查找可出最小对子
    {
        for (int i = point + 1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= 2)
            {
                handCards[i] -= 2;
                return $"2-{i}";
            }
        }
        return "";
    }

    string Search_Triple(int point) //查找可出最小三连张
    {
        for (int i = point + 1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= 3)
            {
                handCards[i] -= 3;
                return $"3-{i}";
            }
        }
        return "";
    }

    string Search_Straight(int point) //查找可出最小顺子
    {
        int curLen = 0;
        for (int i = point + 1; i <= handCards.Count; i++)
        {
            if (handCards[i] > 0)
            {
                curLen++;
            }
            else 
            {
                curLen = 0;
            }
            if (curLen >= 5) 
            {
                for (int j = i; j >i - 5; j--) 
                {
                    handCards[j]--;
                }
                return $"4-{i-4}";
            }
        }
        return "";
    }

    string Search_Three_pairs(int point) //查找可出最小三带对
    {
        int triple_i = -1;
        int pair_i = -1;
        for (int i = point+1; i <= handCards.Count; i++)
        {
            if (triple_i == -1 && handCards[i] >= 3)
            {
                triple_i = i;
            }
            else if (pair_i == -1 && handCards[i] == 2) 
            {
                pair_i = i;
            }
        }
        if (triple_i == -1) //若没有三，失败返回
        {
            return "";
        }
        for (int i = 1; i <= point; i++)
        {
            if (handCards[i] >= 2)
            {
                pair_i = i;
                break;
            }
        }
        if (pair_i == -1) //若没有二，失败返回
        {
            return "";
        }
        handCards[triple_i] -= 3;
        handCards[pair_i] -= 2;
        return $"5-{triple_i}-{pair_i}";
    }

    string Search_Triple_Pairs(int point) //查找可出最小三连对
    {
        int curLen = 0;
        for (int i = point + 1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= 2)
            {
                curLen++;
            }
            else
            {
                curLen = 0;
            }
            if (curLen >= 3)
            {
                for (int j = i; j > i - 3; j--)
                {
                    handCards[j]-=2;
                }
                return $"6-{i - 2}";
            }
        }
        return "";
    }

    string Search_Triple_With_Row(int point) //查找可出最小三同连张
    {
        int curLen = 0;
        for (int i = point + 1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= 3)
            {
                curLen++;
            }
            else
            {
                curLen = 0;
            }
            if (curLen >= 2)
            {
                for (int j = i; j > i - 2; j--)
                {
                    handCards[j] -= 3;
                }
                return $"7-{i - 1}";
            }
        }
        return "";
    }


    string Search_Bomb(int len, int point) //查找可出最小炸弹
    {
        string result = "";
        int result_i = -1;
        for (int i = 1; i <= handCards.Count; i++)
        {
            if (handCards[i] >= len) 
            {
                if (i > point) //长度相同点数更大，一定是最优解
                {
                    handCards[i] -= len;
                    return $"8-{handCards[i]}-{i}";
                }
                else if (handCards[i] > len) //点数不更大但长度更大，如果没找到上面的情况，那么第一个这种情况是最优解
                {
                    if (result.Equals("")) 
                    {
                        result_i = i;
                        result = $"8-{len+1}-{i}";
                    }
                }
            }
        }
        if (result.Equals("")) 
        {
            if (handCards[16] == 2 && handCards[17] == 2) //判断四大天王
            {
                handCards[16] -= 2;
                handCards[17] -= 2;
                return $"8-11-16";
            }
        }
        else
        {
            handCards[result_i] -= len+1;
        }
        return result;
    }

}
