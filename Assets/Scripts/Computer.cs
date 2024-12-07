using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//一个用于电脑玩家的类
//目前简单人机的思路是在游戏开始时把拥有手牌遍历一次，生成手牌库。之后每次出牌只在可打牌型中选择可打的最小牌。遍历手牌库时生成一个(牌点，数量)的字典，代表
public class Computer : MonoBehaviour
{
    Dictionary<int, int> handCards = new Dictionary<int, int>();//手牌

    void Start()
    {
        for (int i = 1; i <= 17; i++) //初始化
        {
            handCards.Add(i, 0);
        }
    }
    void BuildHandCards(Player3D player) //生成手牌
    {
        List<string> cards = new List<string>();
        foreach (Transform child in transform)//获取所有手牌，从小到大排序
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

    void findPlayableCards(Dictionary<string, string> card_on_deck) //根据本轮牌桌上的牌，寻找可打出的最小牌型
    {

    }
        /*
        //开始判断牌型
        bool bingo = true;
        for (int i = 1; i <= 4; i++) //判断四大天王
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
        while (!NoMore) //判断炸弹
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
