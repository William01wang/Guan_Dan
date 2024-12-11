using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CardSet3D : MonoBehaviour //控制牌局进行的全局类
{
    private AssetBundle asset_bundle;
    private List<string> full_cardname;
    public PlayerUIPanel playerUIPanel;
    public float card_ontable_height = 0;
    public int whos_turn = 0;
    
    public Dictionary<string, int> player_rank = new Dictionary<string, int>
        {
            { "Player", 0 },
            { "PlayerLeft", 0 },
            { "PlayerRight", 0 },
            { "PlayerBack", 0 },
        };
    public Dictionary<string, string> card_on_deck = new Dictionary<string, string>
        {
            { "Player", "" },
            { "PlayerLeft", "" },
            { "PlayerRight", "" },
            { "PlayerBack", "" },
        };
    public int n_finished_player = 0;


    void Start()
    {
        asset_bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/cards");
        if (asset_bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle.");
        }
        full_cardname = new List<string>();
        full_cardname.Add("x-LittleJoker");
        full_cardname.Add("x-BigJoker");
        foreach (string suit in new string[] { "Club", "Diamond", "Heart", "Spade" }){
            full_cardname.Add($"{suit}-Ace");
            full_cardname.Add($"{suit}-Jack");
            full_cardname.Add($"{suit}-King");
            full_cardname.Add($"{suit}-Queen");
            for (int i = 2; i <= 10; i++){
                full_cardname.Add($"{suit}-{i}");
            }
        }
        transform.Find("Player").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 0f, 0f));
        transform.Find("PlayerLeft").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 90f, 0f));
        transform.Find("PlayerRight").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, -90f, 0f));
        transform.Find("PlayerBack").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 180f, 0f));

        DistributeCards();

        transform.Find("PlayerLeft").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerLeft").GetComponent<Player3D>());
        transform.Find("PlayerRight").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerRight").GetComponent<Player3D>());
        transform.Find("PlayerBack").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerBack").GetComponent<Player3D>());

        GameObject temp = GameObject.Find("PlayerUIPanel");
        playerUIPanel = temp.GetComponent<PlayerUIPanel>();
    }

    void Update()
    {
        if (n_finished_player >= 4)
        {
            DistributeCards();

            transform.Find("PlayerLeft").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerLeft").GetComponent<Player3D>());
            transform.Find("PlayerRight").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerRight").GetComponent<Player3D>());
            transform.Find("PlayerBack").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards(transform.Find("PlayerBack").GetComponent<Player3D>());
        }
    }

    void DistributeCards() //采用了以单牌的大小排序逐张随机分配的方法，保证发好牌时就是排序好的形式
    {
        System.Random random = new System.Random();
        int card_count = 108;
        int[] random_set = new int[card_count];
        for (int i = 0; i < random_set.Length; i++) 
        {
            random_set[i] = i;
        }
        int index = -1;
        Transform[] child_Players = new Transform[] { transform.Find("Player"), transform.Find("PlayerLeft"), transform.Find("PlayerRight"), transform.Find("PlayerBack") };
        for (int i = 2; i <= 10; i++)//2-10
        {
            foreach (string suit in new string[] { "Club", "Diamond", "Heart", "Spade" })
            {
                index = getRandom(random_set, random, card_count);
                card_count--;
                child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_{suit}{i}"), $"{suit}-{i}");
                index = getRandom(random_set, random, card_count);
                card_count--;
                child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_{suit}{i}"), $"{suit}-{i}");
            }
        }
        foreach (string special_name in new string[] { "Jack", "Queen", "King", "Ace" })//J-A
        {
            foreach (string suit in new string[] { "Club", "Diamond", "Heart", "Spade" })
            {

                index = getRandom(random_set, random, card_count);
                card_count--;
                child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_{suit}{special_name}"), $"{suit}-{special_name}");
                index = getRandom(random_set, random, card_count);
                card_count--;
                child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_{suit}{special_name}"), $"{suit}-{special_name}");
            }
        }
        foreach (string king_name in new string[] { "LittleJoker", "LittleJoker", "BigJoker", "BigJoker" }) //大小王
        {
            index = getRandom(random_set, random, card_count);
            card_count--;
            child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_x{king_name}"), $"x-{king_name}");
        }
        for (int i = 0; i < child_Players.Length; i++) 
        {
            child_Players[i].GetComponent<Player3D>().OrgHands();
            player_rank[child_Players[i].name] = 0;
        }
        n_finished_player = 0;
        whos_turn = 0;
    }

    int getRandom(int[] random_set, System.Random random, int curLen) 
    {
        int index = random.Next(curLen);
        int result = random_set[index];
        random_set[index] = random_set[curLen-1];
        return result / 27;
    }

    public string GetBestCardOnDeck() 
    {
        string best_key = null;
        string best_value = "";
        foreach (var item in card_on_deck)
        {
            if (item.Value == "") //跳过没出牌的玩家
            {
                continue;
            }
            if (best_value.Equals(""))//初始化
            {
                best_key = item.Key;
                best_value = item.Value;
            }
            else
            {
                if (CardSet3D.CompareCard(item.Value, best_value) == 1)
                {
                    best_key = item.Key;
                    best_value = item.Value;
                }
            }
        }
        return best_value;
    }

    public void UpdateCardOnDeck(string new_value, string player) 
    {
        card_on_deck[player] = new_value;
    }


    public static int CompareCard(string card1, string card2) 
    {
        //一个静态public函数，用于牌局中所有类需要的牌大小比较
        //返回值: 1:card1更大 0:card2更大或相同! -1:不可比较
        if (card1.Equals(""))
        {
            return 0;
        }
        if (card2.Equals(""))
        {
            return 1;
        }
        int[] v1 = new int[3];
        int[] v2 = new int[3];
        string[] v1_string = card1.Split("-");
        string[] v2_string = card2.Split("-");
        for (int i = 0; i < v1_string.Length; i++) 
        {
            v1[i] = int.Parse(v1_string[i]);
        }
        for (int i = 0; i < v2_string.Length; i++)
        {
            v2[i] = int.Parse(v2_string[i]);
        }
        if (v1[0] > v2[0]) //v1牌型编号更大(不同牌型)
        {
            if (v1[0] < 8) //v1不是炸弹类，则二者不可比较
            {
                return -1;
            }
            else if (v1[0] == 9)//v1是同花顺，属于特况，要考虑v2若是炸弹长度是否>5
            {
                if (v2[0] == 8 && v2[1] > 5)
                {
                    return 0;
                }
            }
            return 1;
        }
        else if (v1[0] < v2[0]) //v2牌型编号更大(不同牌型)
        {
            if (v2[0] < 8) //v2不是炸弹类，则二者不可比较
            {
                return -1;
            }
            else if (v2[0] == 9)//v2是同花顺，属于特况，要考虑v1若是炸弹长度是否>5
            {
                if (v1[0] == 8 && v1[1] > 5)
                {
                    return 1;
                }
            }
            return 0;
        }
        else //同牌型
        {
            if (v1[0] == 8) //若是炸弹，先看长度再看牌点
            {
                if (v1[1] == v2[1]) //同长度
                {
                    return v1[2] > v2[2] ? 1 : 0;// 比牌点
                }
                else
                {
                    return v1[1] > v2[1] ? 1 : 0;// 比长度
                }
            }
            else //其他牌型直接比牌点
            {
                return v1[1] > v2[1] ? 1 : 0;
            }
        }
    } 
}

