using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class CardSet3D : MonoBehaviour //�����ƾֽ��е�ȫ����
{
    private AssetBundle asset_bundle;
    private List<string> full_cardname;
    private bool resetting = false;
    private bool game_end = false;

    public PlayerUIPanel playerUIPanel;
    public float card_ontable_height = 0;
    public int whos_turn = 0;
    public int top_team = -1;
    public int rank_up = 1;
    public int cur_rank = 2;
    
    public Dictionary<int, int> team_rank = new Dictionary<int, int>
        {
            { 0, 2 },
            { 1, 2 }
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
        GameObject temp = GameObject.Find("PlayerUIPanel");
        playerUIPanel = temp.GetComponent<PlayerUIPanel>();

        BuildCards();

    }

    IEnumerator WaitForNextFrame()//�ȴ�һ֡�����ڵȴ�gameobject��������
    {
        yield return new WaitForEndOfFrame();
    }

    void Update()
    {
        if (n_finished_player >= 3)
        {
            SetRank();
            CheckWin();
            if (game_end)
            {
                resetting = true;
                ReSetPlayers();
                StartCoroutine(WaitForNextFrame());
            }
            else 
            {
                resetting = true;
                ReSetPlayers();
                StartCoroutine(WaitForNextFrame());
            }
            
        }
    }
    private void LateUpdate()
    {
        if (resetting)
        {
            BuildCards();
            resetting = false;
        }
    }

    private void SetRank()
    {
        team_rank[top_team] += rank_up;
        rank_up = 1;
        cur_rank = team_rank[top_team];
        playerUIPanel.Rank.text = "Rank:\r\nTeam 0:\t" + team_rank[0] + "\r\nTeam 1:\t" + team_rank[1] + "\r\nCur_Rank:\t" + cur_rank_tostring();
    }
    private void CheckWin()//��ÿС�ֽ���ʱ������޶���ʤ��
    {
        for (int i = 0; i < 2; i++) 
        {
            if (team_rank[i] >= 15) 
            {
                Debug.Log("team " + i + " win!");//ʤ������ʾ������Ҫ��������ӣ��Ȳ�д.Ҳ������update������
                game_end = true;
            }
        }
    }

    private void ReSetPlayers()
    {
        Player3D Player = transform.Find("Player").GetComponent<Player3D>();
        Player3D PlayerLeft = transform.Find("PlayerLeft").GetComponent<Player3D>();
        Player3D PlayerRight = transform.Find("PlayerRight").GetComponent<Player3D>();
        Player3D PlayerBack = transform.Find("PlayerBack").GetComponent<Player3D>();
        UpdateCardOnDeck("", "Player");
        UpdateCardOnDeck("", "PlayerLeft");
        UpdateCardOnDeck("", "PlayerRight");
        UpdateCardOnDeck("", "PlayerBack");
        Player.ResetPlayingCards();
        Player.ResetHands();
        PlayerLeft.ResetPlayingCards();
        PlayerLeft.ResetHands();
        PlayerRight.ResetPlayingCards();
        PlayerRight.ResetHands();
        PlayerBack.ResetPlayingCards();
        PlayerBack.ResetHands();
    }

    private void BuildCards()
    {
        DistributeCards();

        transform.Find("PlayerLeft").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards();
        transform.Find("PlayerRight").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards();
        transform.Find("PlayerBack").GetComponent<Player3D>().GetComponent<Computer>().BuildHandCards();
    }

    void DistributeCards() //�������Ե��ƵĴ�С���������������ķ�������֤������ʱ��������õ���ʽ
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
        foreach (string king_name in new string[] { "LittleJoker", "LittleJoker", "BigJoker", "BigJoker" }) //��С��
        {
            index = getRandom(random_set, random, card_count);
            card_count--;
            child_Players[index].GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_x{king_name}"), $"x-{king_name}");
        }
        for (int i = 0; i < child_Players.Length; i++) 
        {
            child_Players[i].GetComponent<Player3D>().OrgHands();
        }
        n_finished_player = 0;
        whos_turn = -1;
        NextTrun();
    }

    public void NextTrun()
    {
        whos_turn++;
        switch (whos_turn)
        {
            case 1:
                UpdateCardOnDeck("", "PlayerRight");
                transform.Find("PlayerRight").GetComponent<Player3D>().ResetPlayingCards();
                break;
            case 2:
                UpdateCardOnDeck("", "PlayerBack");
                transform.Find("PlayerBack").GetComponent<Player3D>().ResetPlayingCards();
                break;
            case 3:
                UpdateCardOnDeck("", "PlayerLeft");
                transform.Find("PlayerLeft").GetComponent<Player3D>().ResetPlayingCards();
                break;
            default:
                whos_turn = 0;
                UpdateCardOnDeck("", "Player");
                transform.Find("Player").GetComponent<Player3D>().ResetPlayingCards();
                playerUIPanel.setPanelActive(true);
                foreach (string item in card_on_deck.Values) 
                {
                    Debug.Log(item);
                }
                break;
        }
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
        string best_value = "";
        foreach (var item in card_on_deck)
        {
            if (item.Value == "") //����û���Ƶ����
            {
                continue;
            }
            if (best_value.Equals(""))//��ʼ��
            {
                best_value = item.Value;
            }
            else
            {
                if (CompareCard(item.Value, best_value) == 1)
                {
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


    public int CompareCard(string card1, string card2) 
    {
        //һ��public�����������ƾ�����������Ҫ���ƴ�С�Ƚ�
        //����ֵ: 1:card1���� 0:card2�������ͬ! -1:���ɱȽ�
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
        for (int i = 1; i <= 3; i++) 
        {
            if (v1[0] == i) 
            {
                if (v1[1] == cur_rank) 
                {
                    v1[1] = 15;
                }
            }
            if (v2[0] == i)
            {
                if (v2[1] == cur_rank)
                {
                    v2[1] = 15;
                }
            }
        }
        if (v1[0] == 8)
        {
            if (v1[2] == cur_rank)
            {
                v1[2] = 15;
            }
        }
        if (v2[0] == 8)
        {
            if (v2[2] == cur_rank)
            {
                v2[2] = 15;
            }
        }
        if (v1[0] > v2[0]) //v1���ͱ�Ÿ���(��ͬ����)
        {
            if (v1[0] < 8) //v1����ը���࣬����߲��ɱȽ�
            {
                return -1;
            }
            else if (v1[0] == 9)//v1��ͬ��˳�������ؿ���Ҫ����v2����ը�������Ƿ�>5
            {
                if (v2[0] == 8 && v2[1] > 5)
                {
                    return 0;
                }
            }
            return 1;
        }
        else if (v1[0] < v2[0]) //v2���ͱ�Ÿ���(��ͬ����)
        {
            if (v2[0] < 8) //v2����ը���࣬����߲��ɱȽ�
            {
                return -1;
            }
            else if (v2[0] == 9)//v2��ͬ��˳�������ؿ���Ҫ����v1����ը�������Ƿ�>5
            {
                if (v1[0] == 8 && v1[1] > 5)
                {
                    return 1;
                }
            }
            return 0;
        }
        else //ͬ����
        {
            if (v1[0] == 8) //����ը�����ȿ������ٿ��Ƶ�
            {
                if (v1[1] == v2[1]) //ͬ����
                {
                    return v1[2] > v2[2] ? 1 : 0;// ���Ƶ�
                }
                else
                {
                    return v1[1] > v2[1] ? 1 : 0;// �ȳ���
                }
            }
            else //��������ֱ�ӱ��Ƶ�
            {
                return v1[1] > v2[1] ? 1 : 0;
            }
        }
    }

    string cur_rank_tostring() 
    {
        if (cur_rank==11)
        {
            return "Jack";
        }
        else if (cur_rank == 12)
        {
            return "Queen";
        }
        else if (cur_rank == 13)
        {
            return "King";
        }
        else if (cur_rank == 14)
        {
            return "Ace";
        }
        else
        {
            return cur_rank.ToString();
        }
    }
}

