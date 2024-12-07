using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//һ�����ڵ�����ҵ���
//Ŀǰ���˻���˼·������Ϸ��ʼʱ��ӵ�����Ʊ���һ�Σ��������ƿ⡣֮��ÿ�γ���ֻ�ڿɴ�������ѡ��ɴ����С�ơ��������ƿ�ʱ����һ��(�Ƶ㣬����)���ֵ䣬����
public class Computer : MonoBehaviour
{
    Dictionary<int, int> handCards = new Dictionary<int, int>();//����

    void Start()
    {
        for (int i = 1; i <= 17; i++) //��ʼ��
        {
            handCards.Add(i, 0);
        }
    }
    void BuildHandCards(Player3D player) //��������
    {
        List<string> cards = new List<string>();
        foreach (Transform child in transform)//��ȡ�������ƣ���С��������
        {
            Card3D card_script = child.GetComponent<Card3D>();
            cards.Add(card_script.cardname);
        }
        for (int i = 0; i < cards.Count; i++) //��ѡ���ƵĻ�ɫ�ʹ�С���б�׼��
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

    void findPlayableCards(Dictionary<string, string> card_on_deck) //���ݱ��������ϵ��ƣ�Ѱ�ҿɴ������С����
    {

    }
        /*
        //��ʼ�ж�����
        bool bingo = true;
        for (int i = 1; i <= 4; i++) //�ж��Ĵ�����
        {
            if (points[cards.Count-i] < 16)
            {
                bingo = false; break;
            }
        }
        if (bingo) 
        {
            playables.Add($"8-11-16");
            for (int i = 1; i <= 4; i++)
            {
                points.RemoveAt(cards.Count - i);
                suits.RemoveAt(cards.Count - i);
            }
        }
        bool NoMore = false;
        while (!NoMore) //�ж�ը��
        {
            int curNum = points[0];
            int curCount = 1;
            for (int i = 1; i < points.Count; i++) 
            {
                if (points[i] == curNum)
                {
                    curCount++;
                }
                else 
                {
                   if (curCount >= 4) 
                   {
                        playables.Add($"8-{curCount}-{curNum}");
                       for (int j = i-1; j >= i - curCount; j--) 
                       {
                           points.RemoveAt(j);
                           suits.RemoveAt(j);
                        }
                    }
                }
            }
        }

        */
        


}
