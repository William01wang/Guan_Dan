using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

//һ�����ڵ�����ҵ���
//Ŀǰ���˻���˼·������Ϸ��ʼʱ��ӵ�����Ʊ���һ�Σ��������ƿ⡣֮��ÿ�γ���ֻ�ڿɴ�������ѡ��ɴ����С�ơ��������ƿ�ʱ����һ��(�Ƶ㣬����)���ֵ䣬����
public class Computer : MonoBehaviour
{
    Dictionary<int, int> handCards = new Dictionary<int, int>();//����

    public void BuildHandCards() //��������
    {
        Player3D player = GetComponentInParent<Player3D>();
        handCards.Clear();
        handCards = new Dictionary<int, int>();
        for (int i = 1; i <= 17; i++) //��ʼ��
        {
            handCards.Add(i, 0);
        }
        List<string> cards = new List<string>();
        Debug.Log(player.transform.childCount);
        foreach (Transform child in player.transform)//��ȡ�������ƣ���С��������
        {
            Card3D card_script = child.GetComponent<Card3D>();
            cards.Add(card_script.cardname);
        }
        for (int i = 0; i < cards.Count; i++) //��ѡ���ƵĻ�ɫ�ʹ�С���б�׼��
        {
            string[] cur = cards[i].Split("-");
            if (cur[1].Equals("Jack"))
            {
                handCards[11] = handCards[11] + 1;
            }
            else if (cur[1].Equals("Queen"))
            {
                handCards[12] = handCards[12] + 1;
            }
            else if (cur[1].Equals("King"))
            {
                handCards[13] = handCards[13] + 1;
            }
            else if (cur[1].Equals("Ace"))
            {
                handCards[14] = handCards[14] + 1;
            }
            else if (cur[1].Equals("LittleJoker"))
            {
                handCards[16] = handCards[16] + 1;
            }
            else if (cur[1].Equals("BigJoker"))
            {
                handCards[17] = handCards[17] + 1;
            }
            else
            {
                handCards[int.Parse(cur[1])] = handCards[int.Parse(cur[1])] + 1;
            }
        }
    }

    public string findPlayableCards(CardSet3D cardSet, int cur_rank) //���ݱ��������ϵ��ƣ�Ѱ�ҿɴ������С����
    {
        string result = "";
        string best_value = cardSet.GetBestCardOnDeck();
        if (best_value.Equals("")) //��û�˳����ƣ�ֱ�Ӵ���С������
        {
            if (result.Equals(""))
            {
                result = Search_Triple_With_Row(0);
            }
            if (result.Equals(""))
            {
                result = Search_Three_pairs(0);
            }
            if (result.Equals(""))
            {
                result = Search_Triple_Pairs(0);
            }
            if (result.Equals(""))
            {
                result = Search_Straight(0);
            }
            
            
            if (result.Equals(""))
            {
                if (Random.value > 0.4) 
                {
                    result = Search_Double(0, cur_rank);
                }
            }
            if (result.Equals(""))
            {
                result = Search_Single(0, cur_rank);
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
                if (best_value_int[0] == 8) //���������Ϊը��
                {
                    return Search_Bomb(best_value_int[1], best_value_int[2], cur_rank);
                }
                else if (best_value_int[0] == 9) //���������Ϊͬ��˳
                {
                    return Search_Bomb(6, 0, cur_rank);
                }
                else //���������Ϊ��ͨ����
                {
                    switch (best_value_int[0]) //����ͬ����
                    {
                        case 1:
                            result = Search_Single(best_value_int[1], cur_rank);
                            break;
                        case 2:
                            result = Search_Double(best_value_int[1], cur_rank);
                            break;
                        case 3:
                            result = Search_Triple(best_value_int[1], cur_rank);
                            break;
                        case 4:
                            result = Search_Straight(best_value_int[1]);
                            break;
                        case 5:
                            result = Search_Three_pairs(best_value_int[1]);
                            break;
                        case 6:
                            result = Search_Triple_Pairs(best_value_int[1]);
                            break;
                        case 7:
                            result = Search_Triple_With_Row(best_value_int[1]);
                            break;
                    }
                    if (result.Equals("")) //��ͬ����Ҫ������ը��
                    {
                        result = Search_Bomb(4, 0, cur_rank);
                    }
                   
                }
            }
        return result;
    }

    string Search_Single(int point, int cur_rank) //���ҿɳ���С����
    {
        if (point == cur_rank) 
        {
            point = 15;
        }
        for (int i = point+1; i <= 17; i++)
        {
            if (i == cur_rank)
            {
                continue;
            }
            if (handCards[i] >= 1) 
            {
                handCards[i] = handCards[i] - 1;
                return $"1-{i}";
            }
        }
        if (point < 15) 
        {
            if (handCards[cur_rank] >= 1)
            {
                handCards[cur_rank] = handCards[cur_rank] - 1;
                return $"1-{cur_rank}";
            }
        }
        return "";
    }

    string Search_Double(int point, int cur_rank) //���ҿɳ���С����
    {
        if (point == cur_rank)
        {
            point = 15;
        }
        for (int i = point + 1; i <= 17; i++)
        {
            if (i == cur_rank)
            {
                continue;
            }
            if (handCards[i] >= 2)
            {
                handCards[i] = handCards[i] - 2;
                return $"2-{i}";
            }
        }
        if (point < 15)
        {
            if (handCards[cur_rank] >= 2)
            {
                handCards[cur_rank] = handCards[cur_rank] - 2;
                return $"2-{cur_rank}";
            }
        }
        return "";
    }

    string Search_Triple(int point, int cur_rank) //���ҿɳ���С������
    {
        if (point == cur_rank)
        {
            point = 15;
        }
        for (int i = point + 1; i <= 14; i++)
        {
            if (i == cur_rank) 
            {
                continue;
            }
            if (handCards[i] >= 3)
            {
                handCards[i] = handCards[i] - 3;
                return $"3-{i}";
            }
        }
        if (point < 15)
        {
            if (handCards[cur_rank] >= 3)
            {
                handCards[cur_rank] = handCards[cur_rank] - 3;
                return $"3-{cur_rank}";
            }
        }
        return "";
    }

    string Search_Straight(int point) //���ҿɳ���С˳��
    {
        int curLen = 0;
        for (int i = point + 1; i <= 14; i++)
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
                    handCards[j] = handCards[j] - 1;
                }
                return $"4-{i-4}";
            }
        }
        return "";
    }

    string Search_Three_pairs(int point) //���ҿɳ���С������
    {
        int triple_i = -1;
        int pair_i = -1;
        for (int i = point+1; i <= 14; i++)
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
        if (triple_i == -1) //��û������ʧ�ܷ���
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
        if (pair_i == -1) //��û�ж���ʧ�ܷ���
        {
            return "";
        }
        handCards[triple_i] = handCards[triple_i] - 3;
        handCards[pair_i] = handCards[pair_i] - 2;
        return $"5-{triple_i}-{pair_i}";
    }

    string Search_Triple_Pairs(int point) //���ҿɳ���С������
    {
        int curLen = 0;
        for (int i = point + 1; i <= 14; i++)
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
                    handCards[j] = handCards[j] - 2;
                }
                return $"6-{i - 2}";
            }
        }
        return "";
    }

    string Search_Triple_With_Row(int point) //���ҿɳ���С��ͬ����
    {
        int curLen = 0;
        for (int i = point + 1; i <= 14; i++)
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
                    handCards[j] = handCards[j] - 3;
                }
                return $"7-{i - 1}";
            }
        }
        return "";
    }


    string Search_Bomb(int len, int point, int cur_rank) //���ҿɳ���Сը��
    {
        if (point == cur_rank) 
        {
            point = 15;
        }
        string result = "";
        int result_i = -1;

        for (int i = 1; i <= 14; i++)
        {
            if (i == cur_rank)
            {
                continue;
            }
            if (handCards[i] >= len) 
            {
                if (i > point) //������ͬ��������һ�������Ž�
                {
                    handCards[i] = handCards[i] - len;
                    return $"8-{len}-{i}";
                }
                else if (handCards[i] > len) //���������󵫳��ȸ������û�ҵ�������������ô��һ��������������Ž�
                {
                    if (result.Equals("")) 
                    {
                        result_i = i;
                        result = $"8-{len+1}-{i}";
                    }
                }
            }
        }
        if (point < 15)
        {
            if (handCards[cur_rank] == len)
            {
                handCards[cur_rank] = handCards[cur_rank] - len;
                return $"8-{len}-{cur_rank}";
            }
            else if (handCards[cur_rank] > len)
            {
                if (result.Equals(""))
                {
                    result_i = cur_rank;
                    result = $"8-{len + 1}-{cur_rank}";
                }
            }
        }
        if (result.Equals("")) 
        {
            if (handCards[16] == 2 && handCards[17] == 2) //�ж��Ĵ�����
            {
                handCards[16] -= 2;
                handCards[17] -= 2;
                return $"8-11-16";
            }
        }
        else
        {
            handCards[result_i] = handCards[result_i] - (len + 1);
        }
        return result;
    }

}
